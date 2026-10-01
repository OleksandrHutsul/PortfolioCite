using PortfolioCite.Application.Enums;
using PortfolioCite.Application.Models;
using PortfolioCite.Application.Services;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public partial class ProfileManagementService
{
    private const int MaxFileNameLength = 180;

    public Task<ProfileMediaResult> SaveAvatarAsync(ProfileFileContent file, CancellationToken cancellationToken)
    {
        return SaveFileAsync(ProfileFileKind.Avatar, file, cancellationToken);
    }

    public Task<ProfileMediaResult> RemoveAvatarAsync(CancellationToken cancellationToken)
    {
        return RemoveFileAsync(ProfileFileKind.Avatar, cancellationToken);
    }

    public Task<ProfileMediaResult> SaveResumeAsync(ProfileFileContent file, CancellationToken cancellationToken)
    {
        return SaveFileAsync(ProfileFileKind.Resume, file, cancellationToken);
    }

    public Task<ProfileMediaResult> RemoveResumeAsync(CancellationToken cancellationToken)
    {
        return RemoveFileAsync(ProfileFileKind.Resume, cancellationToken);
    }

    private async Task<ProfileMediaResult> SaveFileAsync(ProfileFileKind kind, ProfileFileContent file, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileForUpdateAsync(cancellationToken);

        if (profile is null)
            return ProfileMediaResult.MissingProfile();

        var validation = Validate(kind, file);

        if (validation is not null)
            return validation;

        var read = await MediaContentInspector.ReadLimitedAsync(file.Content, Limit(kind), cancellationToken);

        if (read.Status == MediaBytesStatus.TooLarge)
            return ProfileMediaResult.TooLarge(SizeError(kind));

        if (read.Status == MediaBytesStatus.Empty || read.Content is null)
            return ProfileMediaResult.Invalid(MediaRules.EmptyFileError);

        var extension = MediaRules.ExtensionOf(file.FileName);
        var headerLength = Math.Min(read.Content.Length, 16);

        if (!MediaContentInspector.HasSignature(extension, read.Content.AsSpan(0, headerLength)))
            return ProfileMediaResult.Invalid(TypeError(kind));

        var stored = await _repository.GetProfileFileForUpdateAsync(profile.Id, kind, cancellationToken);

        if (stored is null)
        {
            stored = new ProfileFile
            {
                ProfileId = profile.Id,
                Kind = kind
            };

            await _repository.AddAsync(stored, cancellationToken);
        }

        stored.FileName = StoredFileName(kind, file.FileName, extension);
        stored.ContentType = MediaRules.ContentTypeFor(extension)!;
        stored.Size = read.Content.LongLength;
        stored.Content = read.Content;
        profile.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return ProfileMediaResult.Success(await MapAsync(profile, cancellationToken));
    }

    private async Task<ProfileMediaResult> RemoveFileAsync(ProfileFileKind kind, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileForUpdateAsync(cancellationToken);

        if (profile is null)
            return ProfileMediaResult.MissingProfile();

        var stored = await _repository.GetProfileFileForUpdateAsync(profile.Id, kind, cancellationToken);

        if (stored is not null)
        {
            _repository.Remove(stored);
            profile.UpdatedAt = DateTimeOffset.UtcNow;

            await _repository.SaveChangesAsync(cancellationToken);
        }

        return ProfileMediaResult.Success(await MapAsync(profile, cancellationToken));
    }

    private static string StoredFileName(ProfileFileKind kind, string fileName, string extension)
    {
        var name = Path.GetFileName(fileName.Trim());

        if (string.IsNullOrWhiteSpace(name) || name is "." or "..")
            name = kind == ProfileFileKind.Avatar ? "avatar" : "resume";

        if (!name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            name += extension;

        if (name.Length <= MaxFileNameLength)
            return name;

        var stemLength = Math.Max(1, MaxFileNameLength - extension.Length);

        return name[..stemLength] + extension;
    }
}
