using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Enums;
using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.Api.Helpers;

public static class ProfileMediaResultMapper
{
    public static ActionResult<ProfileAdminDto> Map(ControllerBase controller, ProfileMediaResult result)
    {
        return result.Failure switch
        {
            ProfileMediaFailure.None when result.Profile is not null => controller.Ok(ProfileMediaLinks.Apply(controller.Request, result.Profile)),
            ProfileMediaFailure.ProfileMissing => controller.NotFound(CreateProblem("Save the profile before uploading files.", StatusCodes.Status404NotFound)),
            ProfileMediaFailure.InvalidFile => controller.BadRequest(CreateProblem(result.Message ?? "The file is not valid.", StatusCodes.Status400BadRequest)),
            ProfileMediaFailure.FileTooLarge => CreateResult(result.Message ?? "The file is too large.", StatusCodes.Status413PayloadTooLarge),
            _ => CreateResult("The file could not be stored. Try again shortly.", StatusCodes.Status500InternalServerError)
        };
    }

    private static ObjectResult CreateResult(string message, int statusCode)
    {
        return new ObjectResult(CreateProblem(message, statusCode))
        {
            StatusCode = statusCode
        };
    }

    private static ProblemDetails CreateProblem(string message, int statusCode)
    {
        return new ProblemDetails
        {
            Title = message,
            Detail = message,
            Status = statusCode
        };
    }
}