using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public partial class TodoEndpoint
    {
        public void AddClearCompletedRoute(IEndpointRouteBuilder app)
        {
            app.MapDelete("/completed", async (ISender sender) =>
            {
                var command = new ClearCompletedCommand();
                var deleteCount = await sender.Send(command);
                return Results.Ok($"Deleted {deleteCount} todos");
            }).RequireAuthorization();
        }
    }

    public sealed record ClearCompletedCommand : UserBoundRequest, IRequest<long>;

    public class ClearCompletedHandler : IRequestHandler<ClearCompletedCommand, long>
    {
        public async Task<long> Handle(ClearCompletedCommand request, CancellationToken cancellationToken)
        {
            var deletedResult =
                await DB.DeleteAsync<TodoItem>(t => t.IsCompleted == true && t.OwnerId == request.UserId,
                    cancellation: cancellationToken);
            return deletedResult.IsAcknowledged switch
            {
                true => deletedResult.DeletedCount,
                _ => 0,
            };
        }
    }
}
