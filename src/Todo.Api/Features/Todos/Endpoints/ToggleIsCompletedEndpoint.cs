using AutoMapper;
using Carter;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public class ToggleIsCompletedEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/todos/{id}/toggle", async ([AsParameters] ToggleIsCompletedCommand command, ISender sender) =>
                await sender.Send(command));
        }
    }

    public sealed record ToggleIsCompletedCommand(string Id) : IRequest<TodoResponse>;

    public class ToggleIsCompletedHandler(IMapper _mapper) : IRequestHandler<ToggleIsCompletedCommand, TodoResponse?>
    {
        public async Task<TodoResponse?> Handle(ToggleIsCompletedCommand request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().OneAsync(request.Id);
            if (todo == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }
            else
            {
                todo.IsCompleted = !todo.IsCompleted;
                todo.CompletedAt = DateTime.UtcNow;
                await todo.SaveAsync();
                return _mapper.Map<TodoResponse>(todo);
            }
        }
    }
}
