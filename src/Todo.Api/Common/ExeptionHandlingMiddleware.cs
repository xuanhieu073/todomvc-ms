using Microsoft.AspNetCore.Mvc;

namespace Todo.Api.Common;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred",
                Extensions =
                {
                    ["errors"] = exception.Errors
                }
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (NotFoundException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Type = "NotFound",
                Title = "Resource not found",
                Detail = "The requested resource could not be found.",
                Extensions =
                {
                    ["errors"] = exception.Errors
                }
            };

            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (UnauthorizedException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Type = "Unauthorized",
                Title = "Unauthorized access",
                Detail = "You do not have permission to access this resource.",
                Extensions =
                {
                    ["errors"] = new[] { exception.Message }
                }
            };

            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        // catch (Exception)
        // {
        //     var problemDetails = new ProblemDetails
        //     {
        //         Status = StatusCodes.Status500InternalServerError,
        //         Type = "InternalServerError",
        //         Title = "Internal server error",
        //         Detail = "An unexpected error has occurred."
        //     };

        //     context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        //     await context.Response.WriteAsJsonAsync(problemDetails);
        // }
    }
}