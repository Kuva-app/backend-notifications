using Kuva.Email.Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kuva.Email.Service.Filters;

public sealed class ValidateModelStateFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var details = context.ModelState
            .SelectMany(x => x.Value?.Errors.Select(error => $"{x.Key}: {error.ErrorMessage}") ?? [])
            .ToList();

        context.Result = new BadRequestObjectResult(new ErrorResponseDto
        {
            Error = "InvalidRequest",
            Message = "The request payload is invalid.",
            Details = details,
            CorrelationId = context.HttpContext.TraceIdentifier
        });
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
