using Carter;
using MediatR;
using Todo.Bff.Features.Todos.Application.Commands;

namespace Todo.Bff.Features.Todos.Endpoints.Delete;

public class ClearCompleted : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/completed", async (ISender sender) =>
        {
            var command = new ClearCompletedCommand();
            return await sender.Send(command);
        });
    }
}