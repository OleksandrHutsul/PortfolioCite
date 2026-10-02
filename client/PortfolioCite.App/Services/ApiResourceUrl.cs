using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Services;

public static class ApiResourceUrl
{
    public static string? Resolve(Uri? apiBase, string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || apiBase is null)
            return url;

        if (url.StartsWith("//", StringComparison.Ordinal) || Uri.TryCreate(url, UriKind.Absolute, out _))
            return url;

        return new Uri(apiBase, url).ToString();
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
