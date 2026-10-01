namespace PortfolioCite.Application.Services.Projects;

public static class ProjectImageLocations
{
    public static string Route(int projectId, string fileName, DateTimeOffset version)
    {
        return $"/api/portfolio/projects/{projectId}/image/{Uri.EscapeDataString(fileName)}?v={version.ToUnixTimeSeconds()}";
    }
}
