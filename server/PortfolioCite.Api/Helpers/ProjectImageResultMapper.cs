using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Enums;
using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Api.Helpers;

public static class ProjectImageResultMapper
{
    public static ActionResult<ProjectDto> Map(ControllerBase controller, ProjectImageResult result)
    {
        if (result.Failure == ProjectImageFailure.None && result.Project is not null)
            return controller.Ok(ProjectMediaLinks.Apply(controller.Request, result.Project));

        var (status, message) = result.Failure switch
        {
            ProjectImageFailure.ProjectMissing => (404, "The project was not found."),
            ProjectImageFailure.InvalidFile => (400, result.Message ?? "The file is not valid."),
            ProjectImageFailure.FileTooLarge => (413, result.Message ?? "The file is too large."),
            _ => (500, "The image could not be stored. Try again shortly.")
        };

        return new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = message,
            Detail = message
        })
        {
            StatusCode = status
        };
    }
}
