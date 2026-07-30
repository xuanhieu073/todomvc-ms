using Carter;
using MediatR;

namespace Todo.Api.Features.Reminders.Endpoints.Update;

public class DimissReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/reminders/{id}/dimiss", async (string Id, ISender sender) =>
        {
            var command = new DimissReminderCommand(Id);
            return await sender.Send(command);
        });
    }
}