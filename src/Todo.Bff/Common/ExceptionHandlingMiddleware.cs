using Microsoft.AspNetCore.Mvc;

namespace Todo.Bff.Common;

class Error
{
    public List<ValidationError>? errors { get; set; }
}
public record ValidationError(string propertyName, string errorMessage);

public sealed class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }

    public IEnumerable<ValidationError> Errors { get; }
}

public sealed class NotFoundException : Exception
{
    public NotFoundException(IEnumerable<ValidationError> errors)
        : base("The requested resource could not be found.")
    {
        Errors = errors;
    }

    public IEnumerable<ValidationError> Errors { get; }
}

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
                Detail = "One or more validation errors has occurred"
            };

            if (exception.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exception.Errors;
            }

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
                Detail = "The requested resource could not be found."
            };

            if (exception.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exception.Errors;
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception exception)
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