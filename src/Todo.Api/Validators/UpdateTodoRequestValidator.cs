using FluentValidation;
using Todo.Api.DTOs;

namespace Todo.Api.Validators
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
