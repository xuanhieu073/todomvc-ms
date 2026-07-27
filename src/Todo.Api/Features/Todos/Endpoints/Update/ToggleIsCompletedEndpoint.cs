using Carter;
using MediatR;
using Todo.Api.Features.Todos.Application.Commands;

namespace Todo.Api.Features.Todos.Endpoints.Update
{
    public class ToggleIsCompletedEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/todos/{id}/toggle", async (string Id, ISender sender) => {
                var command = new ToggleIsCompletedCommand(Id);
                var result = await sender.Send(command);
                return result switch
                {
                    null => Results.NotFound("Invalid Id"),
                    _ => Results.Ok(result)
                };
            });
        }
    }
}
