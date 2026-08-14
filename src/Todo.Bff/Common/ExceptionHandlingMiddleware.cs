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

public sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }

        catch (ValidationException exception)
        {
            logger.LogWarning(exception, "Validation error occurred.");
            await HandleExceptionAsync(context, exception, StatusCodes.Status400BadRequest, "ValidationFailure",
                "Validation error", "One or more validation errors has occurred", exception.Errors);
        }
        catch (NotFoundException exception)
        {
            logger.LogWarning(exception, "Resource not found.");
            await HandleExceptionAsync(context, exception, StatusCodes.Status404NotFound, "NotFound",
                "Resource not found", "The requested resource could not be found.", exception.Errors);
        }
        catch (UnauthorizedException exception)
        {
            logger.LogWarning(exception, "Unauthorized access attempt.");
            await HandleExceptionAsync(context, exception, StatusCodes.Status401Unauthorized, "Unauthorized",
                "Unauthorized", "You must provide valid authentication credentials to access this resource.",
                exception.Message);
        }
        catch (ClientErrorException exception)
        {
            logger.LogWarning(exception, "Client error occurred with status code {StatusCode}.", exception.StatusCode);
            int statusCode = (int)exception.StatusCode;
            await HandleExceptionAsync(context, exception, statusCode, exception.StatusCode.ToString(), "Client Error",
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception occurred on the server.");
            await HandleExceptionAsync(context, exception, StatusCodes.Status500InternalServerError,
                "InternalServerError", "Internal server error", "An unexpected error has occurred.");
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        string type,
        string title,
        string detail,
        object? errors = null)
    {
        if (context.Response.HasStarted)
        {
            logger.LogWarning(
                "The response has already started, the exception middleware cannot modify the response options.");
            throw exception;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = type,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors != null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}