using PortfolioCite.Application.Enums;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Application.Models;

public record ProjectImageResult(ProjectDto? Project, ProjectImageFailure Failure, string? Message)
{
    public static ProjectImageResult Success(ProjectDto project)
    {
        return new ProjectImageResult(project, ProjectImageFailure.None, null);
    }

    public static ProjectImageResult Missing()
    {
        return new ProjectImageResult(null, ProjectImageFailure.ProjectMissing, null);
    }

    public static ProjectImageResult Invalid(string message)
    {
        return new ProjectImageResult(null, ProjectImageFailure.InvalidFile, message);
    }

    public static ProjectImageResult TooLarge(string message)
    {
        return new ProjectImageResult(null, ProjectImageFailure.FileTooLarge, message);
    }
}
