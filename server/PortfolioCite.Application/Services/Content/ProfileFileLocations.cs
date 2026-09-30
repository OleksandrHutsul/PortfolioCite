using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public static class ProfileFileLocations
{
    public const string AvatarRoute = "/api/portfolio/avatar";
    public const string ResumeRoute = "/api/portfolio/resume";

    public static string? Avatar(IEnumerable<ProfileFile> files, DateTimeOffset version)
    {
        return Link(AvatarRoute, files, ProfileFileKind.Avatar, version);
    }

    public static string? Resume(IEnumerable<ProfileFile> files, DateTimeOffset version)
    {
        return Link(ResumeRoute, files, ProfileFileKind.Resume, version);
    }

    private static string? Link(string route, IEnumerable<ProfileFile> files, ProfileFileKind kind, DateTimeOffset version)
    {
        var file = files.FirstOrDefault(item => item.Kind == kind);

        if (file is null || string.IsNullOrWhiteSpace(file.FileName))
            return null;

        return $"{route}/{Uri.EscapeDataString(file.FileName)}?v={version.ToUnixTimeSeconds()}";
    }
}
