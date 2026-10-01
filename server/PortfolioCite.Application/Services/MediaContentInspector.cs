using PortfolioCite.Application.Enums;
using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.Application.Services;

public static class MediaContentInspector
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    public static bool ContentTypeMatches(string extension, string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        var separator = contentType.IndexOf(';');
        var mediaType = (separator >= 0 ? contentType[..separator] : contentType).Trim();
        var expected = MediaRules.ContentTypeFor(extension);

        return expected is not null && mediaType.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasSignature(string extension, ReadOnlySpan<byte> header)
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

    public static async Task<MediaBytes> ReadLimitedAsync(Stream source, long maxBytes, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        using var destination = new MemoryStream();

        while (true)
        {
            var read = await source.ReadAsync(buffer, cancellationToken);

            if (read == 0)
                break;

            if (destination.Length + read > maxBytes)
                return new MediaBytes(null, MediaBytesStatus.TooLarge);

            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }

        if (destination.Length == 0)
            return new MediaBytes(null, MediaBytesStatus.Empty);

        return new MediaBytes(destination.ToArray(), MediaBytesStatus.Ok);
    }
}
