using AutoMapper;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public partial class TodoEndpoint
    {
        public void AddToggleCompletedRoute(IEndpointRouteBuilder app)
        {
            app.MapPatch("/{id}/toggle",
                async ([AsParameters] ToggleIsCompletedCommand command, ISender sender) =>
                await sender.Send(command)).RequireAuthorization();
        }
    }

    public sealed record ToggleIsCompletedCommand(string Id) : UserBoundRequest, IRequest<TodoResponse>;

    public class ToggleIsCompletedHandler(IMapper mapper) : IRequestHandler<ToggleIsCompletedCommand, TodoResponse?>
    {
        public async Task<TodoResponse?> Handle(ToggleIsCompletedCommand request, CancellationToken cancellationToken)
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
                todo.IsCompleted = !todo.IsCompleted;
                todo.CompletedAt = DateTime.UtcNow;
                await todo.SaveAsync(cancellation: cancellationToken);
                return mapper.Map<TodoResponse>(todo);
            }
        }
    }
}
