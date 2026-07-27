using Carter;
using MediatR;
using Todo.Api.Features.Todos.Application.Commands;

namespace Todo.Api.Features.Todos.Endpoints.Delete
{
    public class ClearCompletedEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/todos/completed", async (ISender sender) => {
                var command = new ClearCompletedCommand();
                var deleteCount = await sender.Send(command);
                return Results.Ok($"Deleted {deleteCount} todos");
            });
        }
    }
}
