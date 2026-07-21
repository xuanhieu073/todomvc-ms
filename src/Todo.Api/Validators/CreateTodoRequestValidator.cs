using FluentValidation;
using Todo.Api.DTOs;

namespace Todo.Api.Validators
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
