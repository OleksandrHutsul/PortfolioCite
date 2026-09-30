using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.Api.Helpers;

public static class ProfileFileValidator
{
    public static ActionResult? Validate(IFormFile? file, long maxBytes, string sizeError)
    {
        if (file is null || file.Length <= 0)
            return BadRequest(ProfileMediaRules.EmptyFileError);

        if (file.Length > maxBytes)
            return StatusCode(sizeError, StatusCodes.Status413PayloadTooLarge);

        return null;
    }

    private static BadRequestObjectResult BadRequest(string message)
    {
        return new BadRequestObjectResult(CreateProblem(message, StatusCodes.Status400BadRequest));
    }

    private static ObjectResult StatusCode(string message, int statusCode)
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