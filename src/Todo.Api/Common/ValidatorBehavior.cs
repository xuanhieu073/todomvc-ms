using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Todo.Api.Common;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context))
        );

        var errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(validationFailure.PropertyName, validationFailure.ErrorMessage))
            .ToList();

        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        var response = await next();

        return response;
    }
}

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

public record ValidationError(string propertyName, string errorMessage);