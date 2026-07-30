using Carter;
using MediatR;
using Todo.Bff.Features.Todos.Application.Commands;

namespace Todo.Bff.Features.Todos.Endpoints.Update;

public class ToggleIsCompletedEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/bff/todos/{id}", async (string Id, ISender sender) =>
        {
            var command = new ToggleIsCompletedCommand(Id);
            var response = await sender.Send(command);
            return response.ToHttpResponse();
        });
    }
}