using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Services;

public static class ApiResourceUrl
{
    public static string? Resolve(Uri? apiBase, string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || IsAlreadyAddressable(url))
            return url;

        if (IsApiPath(url))
        {
            if (apiBase is null)
                return url;

            var path = url.StartsWith('/') ? url : $"/{url}";

            return apiBase.GetLeftPart(UriPartial.Authority) + path;
        }

        return url.StartsWith('/') ? url : $"/{url}";
    }

    private static bool IsApiPath(string url)
    {
        return url.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) || url.StartsWith("api/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAlreadyAddressable(string url)
    {
        return url.StartsWith("data:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("blob:", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("//", StringComparison.Ordinal) || url.Contains("://", StringComparison.Ordinal);
    }

    public static PortfolioSnapshotDto Resolve(Uri? apiBase, PortfolioSnapshotDto snapshot)
    {
        return snapshot with
        {
            Profile = snapshot.Profile is null ? null : Resolve(apiBase, snapshot.Profile),
            Projects = snapshot.Projects.Select(project => Resolve(apiBase, project)).ToList()
        };
    }

    public static ProfileDto Resolve(Uri? apiBase, ProfileDto profile)
    {
        return profile with
        {
            AvatarUrl = Resolve(apiBase, profile.AvatarUrl),
            ResumeUrl = Resolve(apiBase, profile.ResumeUrl)
        };
    }

    public static ProfileAdminDto Resolve(Uri? apiBase, ProfileAdminDto profile)
    {
        return profile with
        {
            AvatarUrl = Resolve(apiBase, profile.AvatarUrl),
            ResumeUrl = Resolve(apiBase, profile.ResumeUrl)
        };
    }

    public static ProjectDto Resolve(Uri? apiBase, ProjectDto project)
    {
        return project with
        {
            ImageUrl = Resolve(apiBase, project.ImageUrl)
        };
    }

    public static IReadOnlyList<ProjectDto> Resolve(Uri? apiBase, IReadOnlyList<ProjectDto> projects)
    {
        return projects.Select(project => Resolve(apiBase, project)).ToList();
    }
}
