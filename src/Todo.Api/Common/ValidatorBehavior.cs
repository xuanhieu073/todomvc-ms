using FluentValidation;
using MediatR;

namespace Todo.Api.Common;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken))
        );

        var errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure =>
                new ValidationError(validationFailure.PropertyName, validationFailure.ErrorMessage))
            .ToList();

        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        var response = await next();

        return response;
    }
}

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

public sealed class UnauthorizedException(string error) : Exception(error);

public record ValidationError(string PropertyName, string ErrorMessage);