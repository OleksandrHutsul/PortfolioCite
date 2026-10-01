using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PortfolioCite.Application.Validation;

namespace PortfolioCite.Api.Filters;

public class ContentValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ContentValidationException exception) return;

        context.Result = new BadRequestObjectResult(new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest
        });

        context.ExceptionHandled = true;
    }
}
