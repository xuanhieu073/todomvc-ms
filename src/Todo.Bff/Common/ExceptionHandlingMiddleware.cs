using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Todo.Bff.Common;

class Error
{
    public List<ValidationError>? Errors { get; set; }
}

public record ValidationError(string PropertyName, string ErrorMessage);

public sealed class ValidationException(IEnumerable<ValidationError> errors)
    : Exception("One or more validation failures have occurred.")
{
    public IEnumerable<ValidationError> Errors { get; } = errors;
}

public sealed class NotFoundException(IEnumerable<ValidationError> errors)
    : Exception("The requested resource could not be found.")
{
    public IEnumerable<ValidationError> Errors { get; } = errors;
}

public class ClientErrorException(string errorMessage, HttpStatusCode statusCode)
    : Exception(errorMessage)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public sealed class UnauthorizedException(string errorMessage)
    : Exception(errorMessage);

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
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
                Title = "Unauthorized",
                Detail = "You must provide valid authentication credentials to access this resource.",
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", exception.Message }
                }
            };

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (ClientErrorException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)exception.StatusCode,
                Type = exception.StatusCode.ToString(),
                Title = "CLient Error",
                Detail = exception.Message
            };

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "InternalServerError",
                Title = "Internal server error",
                Detail = "An unexpected error has occurred."
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}