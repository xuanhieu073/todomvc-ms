using AutoMapper;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public partial class TodoEndpoint
    {
        public void AddCreateTodoRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("", async (CreateTodoCommand command, ISender sender) =>
                await sender.Send(command));
        }
    }

    public sealed record CreateTodoCommand(string Title, DateTime DueAt) : UserBoundRequest, IRequest<TodoResponse>;

    public class CreateTodoHandler(IMapper mapper) : IRequestHandler<CreateTodoCommand, TodoResponse>
    {
        public async Task<TodoResponse> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
        {
            var newTodo = new TodoItem
                { OwnerId = command.UserId, Title = command.Title, DueAt = command.DueAt, CreatedAt = DateTime.UtcNow };
            await newTodo.SaveAsync(cancellation: cancellationToken);
            return mapper.Map<TodoResponse>(newTodo);
        }
    }

    public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
    {
        public CreateTodoCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.DueAt).NotNull();
        }
    }
}
