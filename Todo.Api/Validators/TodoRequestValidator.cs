using FluentValidation;
using Todo.Api.DTOs;

namespace Todo.Api.Validators
{
    public class TodoRequestValidator : AbstractValidator<CreateToDoRequest>
    {
        public TodoRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(200);
        }
    }
}
