using AutoMapper;
using Carter;
using FluentValidation;
using MediatR;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos.Endpoints
{
    public class CreateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/todos", async (CreateTodoCommand command, ISender sender) =>
                await sender.Send(command));
        }
    }

    public sealed record CreateTodoCommand(string Title, DateTime DueAt) : IRequest<TodoResponse>;

    public class CreateTodoHandler(IMapper mapper) : IRequestHandler<CreateTodoCommand, TodoResponse>
    {
        public async Task<TodoResponse> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
        {
            var newTodo = new TodoItem { Title = command.Title, DueAt = command.DueAt, CreatedAt = DateTime.UtcNow };
            await newTodo.SaveAsync();
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
