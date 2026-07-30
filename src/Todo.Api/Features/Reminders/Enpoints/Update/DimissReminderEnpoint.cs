using Carter;
using MediatR;

namespace Todo.Api.Features.Reminders.Endpoints.Update;

public class DimissReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reminders/{id}/dimiss", (string Id, ISender sender) =>
        {
            var command = new DimissReminderCommand(Id);
            return sender.Send(command);
        });
    }
}