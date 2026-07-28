using FluentValidation;
using Todo.Api.Features.Todos.Application.Commands;

namespace Todo.Api.Features.Todos.Validators;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(x => x.updateTodoRequest.Title).NotEmpty();
        RuleFor(x => x.updateTodoRequest.Title).MaximumLength(200);
        RuleFor(x => x.updateTodoRequest.isCompleted).NotNull();
    }
}