using PortfolioCite.Application.Enums;
using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Projects;

public partial class ProjectService
{
    private const int MaxFileNameLength = 180;
    private const int MaxImageUrlLength = 500;

    public async Task<ProjectImageResult> SaveImageAsync(int projectId, ProfileFileContent file, CancellationToken cancellationToken)
    {
        var project = await _repository.GetProjectForUpdateAsync(projectId, cancellationToken);

        if (project is null)
            return ProjectImageResult.Missing();

        var validation = ValidateImage(file);

        if (validation is not null)
            return validation;

        var read = await MediaContentInspector.ReadLimitedAsync(file.Content, MediaRules.ImageMaxBytes, cancellationToken);

        if (read.Status == MediaBytesStatus.TooLarge)
            return ProjectImageResult.TooLarge(MediaRules.ImageSizeError);

        if (read.Status == MediaBytesStatus.Empty || read.Content is null)
            return ProjectImageResult.Invalid(MediaRules.EmptyFileError);

        var extension = MediaRules.ExtensionOf(file.FileName);
        var headerLength = Math.Min(read.Content.Length, 16);

        if (!MediaContentInspector.HasSignature(extension, read.Content.AsSpan(0, headerLength)))
            return ProjectImageResult.Invalid(MediaRules.ImageTypeError);

        var stored = await _repository.GetProjectImageForUpdateAsync(projectId, cancellationToken);

        if (stored is null)
        {
            stored = new ProjectImage
            {
                ProjectId = projectId
            };

            await _repository.AddAsync(stored, cancellationToken);
        }

        var now = DateTimeOffset.UtcNow;

        stored.FileName = StoredFileName(file.FileName, extension);
        stored.ContentType = MediaRules.ContentTypeFor(extension)!;
        stored.Size = read.Content.LongLength;
        stored.Content = read.Content;

        project.ImageUrl = ImageReference(project.Id, stored.FileName, extension, now);
        project.UpdatedAt = now;

        await _repository.SaveChangesAsync(cancellationToken);

        return ProjectImageResult.Success(Map(project));
    }

    public async Task<ProjectImageResult> RemoveImageAsync(int projectId, CancellationToken cancellationToken)
    {
        var project = await _repository.GetProjectForUpdateAsync(projectId, cancellationToken);

        if (project is null)
            return ProjectImageResult.Missing();

        var stored = await _repository.GetProjectImageForUpdateAsync(projectId, cancellationToken);

        if (stored is null && project.ImageUrl is null)
            return ProjectImageResult.Success(Map(project));

        if (stored is not null)
            _repository.Remove(stored);

        project.ImageUrl = null;
        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return ProjectImageResult.Success(Map(project));
    }

    private static ProjectImageResult? ValidateImage(ProfileFileContent file)
    {
        if (file.Length <= 0)
            return ProjectImageResult.Invalid(MediaRules.EmptyFileError);

        if (file.Length > MediaRules.ImageMaxBytes)
            return ProjectImageResult.TooLarge(MediaRules.ImageSizeError);

        var extension = MediaRules.ExtensionOf(file.FileName);

        if (!MediaRules.IsImageExtension(extension) || !MediaContentInspector.ContentTypeMatches(extension, file.ContentType))
            return ProjectImageResult.Invalid(MediaRules.ImageTypeError);

        return null;
    }

    private static string ImageReference(int projectId, string fileName, string extension, DateTimeOffset version)
    {
        var route = ProjectImageLocations.Route(projectId, fileName, version);

        if (route.Length <= MaxImageUrlLength)
            return route;

        return ProjectImageLocations.Route(projectId, "image" + extension, version);
    }

    private static string StoredFileName(string fileName, string extension)
    {
        var name = Path.GetFileName(fileName.Trim());

        if (string.IsNullOrWhiteSpace(name) || name is "." or "..")
            name = "image" + extension;

        if (!name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            name += extension;

        if (name.Length <= MaxFileNameLength)
            return name;

        var stemLength = Math.Max(1, MaxFileNameLength - extension.Length);

        return name[..stemLength] + extension;
    }
}
