using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Api.Helpers;

public static class ProjectMediaLinks
{
    public static ProjectDto Apply(HttpRequest request, ProjectDto project)
    {
        return project with
        {
            ImageUrl = MediaLinks.Absolute(request, project.ImageUrl)
        };
    }

    public static IReadOnlyList<ProjectDto> Apply(HttpRequest request, IReadOnlyList<ProjectDto> projects)
    {
        return projects.Select(project => Apply(request, project)).ToList();
    }

    public static PortfolioSnapshotDto Apply(HttpRequest request, PortfolioSnapshotDto snapshot)
    {
        return snapshot with
        {
            Projects = Apply(request, snapshot.Projects)
        };
    }
}
