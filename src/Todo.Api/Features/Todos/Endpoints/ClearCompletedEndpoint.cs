using Carter;
using MediatR;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos.Endpoints
{
    public class ClearCompletedEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/todos/completed", async (ISender sender) =>
            {
                var command = new ClearCompletedCommand();
                var deleteCount = await sender.Send(command);
                return Results.Ok($"Deleted {deleteCount} todos");
            });
        }
    }

    public sealed record ClearCompletedCommand() : IRequest<long>;

    public class ClearCompletedHandler : IRequestHandler<ClearCompletedCommand, long>
    {
        public async Task<long> Handle(ClearCompletedCommand request, CancellationToken cancellationToken)
        {
            var deletedResult =
                await DB.DeleteAsync<TodoItem>(todo => todo.IsCompleted == true, cancellation: cancellationToken);
            return deletedResult.IsAcknowledged switch
            {
                true => deletedResult.DeletedCount,
                _ => 0,
            };
        }
    }
}
