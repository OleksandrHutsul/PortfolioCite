using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public partial class ProfileManagementService
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private static ProfileMediaResult? Validate(ProfileFileKind kind, ProfileFileContent file)
    {
        if (file.Length <= 0)
            return ProfileMediaResult.Invalid(ProfileMediaRules.EmptyFileError);

        if (file.Length > Limit(kind))
            return ProfileMediaResult.TooLarge(SizeError(kind));

        var extension = ProfileMediaRules.ExtensionOf(file.FileName);

        if (kind == ProfileFileKind.Avatar && !ProfileMediaRules.IsAvatarExtension(extension))
            return ProfileMediaResult.Invalid(ProfileMediaRules.AvatarTypeError);

        if (kind == ProfileFileKind.Resume && !ProfileMediaRules.IsResumeExtension(extension))
            return ProfileMediaResult.Invalid(ProfileMediaRules.ResumeTypeError);

        if (!ContentTypeMatches(extension, file.ContentType))
            return ProfileMediaResult.Invalid(TypeError(kind));

        return null;
    }

    private static async Task<(ProfileMediaResult? Failure, long Length)> CopyLimitedAsync(Stream source, Stream destination, long maxBytes, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long total = 0;

        while (true)
        {
            var read = await source.ReadAsync(buffer, cancellationToken);

            if (read == 0)
                break;

            total += read;

            if (total > maxBytes)
                return (ProfileMediaResult.TooLarge(maxBytes == ProfileMediaRules.AvatarMaxBytes ? ProfileMediaRules.AvatarSizeError : ProfileMediaRules.ResumeSizeError), total);

            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }

        if (total == 0)
            return (ProfileMediaResult.Invalid(ProfileMediaRules.EmptyFileError), 0);

        return (null, total);
    }

    private static bool ContentTypeMatches(string extension, string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        var separator = contentType.IndexOf(';');
        var mediaType = (separator >= 0 ? contentType[..separator] : contentType).Trim();
        var expected = ProfileMediaRules.ContentTypeFor(extension);

        return expected is not null && mediaType.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasSignature(string extension, ReadOnlySpan<byte> header)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" => header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => header.Length >= PngSignature.Length && header.StartsWith(PngSignature),
            ".webp" => header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header.Slice(8, 4).SequenceEqual("WEBP"u8),
            ".pdf" => header.Length >= 5 && header[..5].SequenceEqual("%PDF-"u8),
            _ => false
        };
    }

    private static long Limit(ProfileFileKind kind)
    {
        return kind == ProfileFileKind.Avatar ? ProfileMediaRules.AvatarMaxBytes : ProfileMediaRules.ResumeMaxBytes;
    }

    private static string TypeError(ProfileFileKind kind)
    {
        return kind == ProfileFileKind.Avatar ? ProfileMediaRules.AvatarTypeError : ProfileMediaRules.ResumeTypeError;
    }

    private static string SizeError(ProfileFileKind kind)
    {
        return kind == ProfileFileKind.Avatar ? ProfileMediaRules.AvatarSizeError : ProfileMediaRules.ResumeSizeError;
    }
}
