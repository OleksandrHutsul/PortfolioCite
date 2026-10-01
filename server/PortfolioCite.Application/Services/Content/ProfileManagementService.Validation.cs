using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public partial class ProfileManagementService
{
    private static ProfileMediaResult? Validate(ProfileFileKind kind, ProfileFileContent file)
    {
        if (file.Length <= 0)
            return ProfileMediaResult.Invalid(MediaRules.EmptyFileError);

        if (file.Length > Limit(kind))
            return ProfileMediaResult.TooLarge(SizeError(kind));

        var extension = MediaRules.ExtensionOf(file.FileName);

        if (kind == ProfileFileKind.Avatar && !MediaRules.IsImageExtension(extension))
            return ProfileMediaResult.Invalid(MediaRules.ImageTypeError);

        if (kind == ProfileFileKind.Resume && !ProfileMediaRules.IsResumeExtension(extension))
            return ProfileMediaResult.Invalid(ProfileMediaRules.ResumeTypeError);

        if (!MediaContentInspector.ContentTypeMatches(extension, file.ContentType))
            return ProfileMediaResult.Invalid(TypeError(kind));

        return null;
    }

    private static long Limit(ProfileFileKind kind)
    {
        return kind == ProfileFileKind.Avatar ? MediaRules.ImageMaxBytes : ProfileMediaRules.ResumeMaxBytes;
    }

    private static string TypeError(ProfileFileKind kind)
    {
        return kind == ProfileFileKind.Avatar ? MediaRules.ImageTypeError : ProfileMediaRules.ResumeTypeError;
    }

    private static string SizeError(ProfileFileKind kind)
    {
        return kind == ProfileFileKind.Avatar ? MediaRules.ImageSizeError : ProfileMediaRules.ResumeSizeError;
    }
}
