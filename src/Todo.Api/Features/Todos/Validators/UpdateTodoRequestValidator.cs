using FluentValidation;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Validators
{
    public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoRequest>
    {
        public UpdateTodoRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(200);
            RuleFor(x => x.isCompleted).NotNull();
        }
    }
}
