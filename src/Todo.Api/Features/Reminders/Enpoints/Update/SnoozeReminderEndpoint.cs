using Carter;
using MediatR;
using Todo.Api.Features.Reminders.Application.Commands;

namespace Todo.Api.Features.Reminders.Endpoints.Update;

public class SnoozeReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/reminders/{id}/snooze", (string Id, ISender sender) =>
        {
            var command = new SnoozeReminderCommand(Id);
            return sender.Send(command);
        });
    }
}