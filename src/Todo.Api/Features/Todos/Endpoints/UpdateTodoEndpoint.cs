using AutoMapper;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public partial class TodoEndpoint
    {
        public void AddUpateTodoRoute(IEndpointRouteBuilder app)
        {
            app.MapPut("/{id}", async (string Id, UpdateTodoCommand command, ISender sender) =>
                await sender.Send(command with { Id = Id }));
        }
    }

    public sealed record UpdateTodoCommand(string Id, string Title, bool IsCompleted, DateTime DueAt)
        : UserBoundRequest, IRequest<TodoResponse?>;

    public class UpdateTodoHandler(IMapper mapper) : IRequestHandler<UpdateTodoCommand, TodoResponse?>
    {
        public async Task<TodoResponse?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().Match(t => t.ID == request.Id && t.OwnerId == request.UserId)
                .ExecuteFirstAsync(cancellationToken);
            if (todo == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }
            else
            {
                mapper.Map(request, todo);
                await todo.SaveAsync(cancellation: cancellationToken);
                return mapper.Map<TodoResponse>(todo);
            }
        }
    }

    public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.IsCompleted).NotNull();
            RuleFor(x => x.DueAt).NotNull();
        }
    }
}
