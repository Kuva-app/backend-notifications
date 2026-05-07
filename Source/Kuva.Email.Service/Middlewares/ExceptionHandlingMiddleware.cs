using Kuva.Email.Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Kuva.Email.Service.Middlewares;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499;
        }
        catch (ArgumentException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "InvalidRequest", ex.Message, logger);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, "Conflict", ex.Message, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled request failure.");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "UnexpectedError", "An unexpected error occurred.", logger);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string error, string message, ILogger logger)
    {
        logger.LogWarning("Request failed with {Error}.", error);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = error,
            Detail = message,
            Instance = context.Request.Path
        };
        problem.Extensions["correlationId"] = context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(problem, context.RequestAborted);
    }
}
