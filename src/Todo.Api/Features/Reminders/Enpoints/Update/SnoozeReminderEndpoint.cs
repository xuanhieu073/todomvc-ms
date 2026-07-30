using Carter;
using MediatR;
using Todo.Api.Features.Reminders.Application.Commands;

namespace Todo.Api.Features.Reminders.Endpoints.Update;

public class SnoozeReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/reminders/{id}/snooze", (string Id, SnoozeReminderRequest request, ISender sender) =>
        {
            var command = new SnoozeReminderCommand(Id, request.minutes);
            return sender.Send(command);
        });
    }
}