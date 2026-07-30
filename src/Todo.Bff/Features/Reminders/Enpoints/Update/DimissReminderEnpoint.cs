using Carter;
using MediatR;
using Todo.Bff.Features.Reminders.Application.Commands;

namespace Todo.Bff.Features.Reminders.Endpoint.Update;

public class DimissReminderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/bff/reminders/{id}/dimiss", async (string Id, ISender sender) =>
        {
            var command = new DimissReminderCommand(Id);
            var response = await sender.Send(command);
            return response.ToHttpResponse();
        });
    }
}