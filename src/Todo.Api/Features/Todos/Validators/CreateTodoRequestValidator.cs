using FluentValidation;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Validators
{
    public class CreateTodoRequestValidator : AbstractValidator<CreateToDoRequest>
    {
        public CreateTodoRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(200);
        }
    }
}
