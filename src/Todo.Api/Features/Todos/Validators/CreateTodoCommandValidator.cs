using FluentValidation;
using Todo.Api.Features.Todos.Application.Commands;

namespace Todo.Api.Features.Todos.Validators;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.createTodoRequest.Title).NotEmpty();
    }
}