using Carter;
using MediatR;
using Todo.Bff.Features.Reminders.Application.Commands;
using Todo.Bff.Features.Reminders.DTOs;

namespace Todo.Bff.Features.Reminders.Endpoint.Update;

public class SnoozeReminderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/bff/reminders/{id}/snooze", async (string Id, SnoozeReminderReuqest request, ISender sender) =>
        {
            var command = new SnoozeReminderCommand(Id, request);
            var response = await sender.Send(command);
            return response.ToHttpResponse();
        });
    }
}