using Carter;
using MediatR;
using Todo.Api.Features.Reminders.Application.Commands;

namespace Todo.Bff.Features.Reminders.Endpoints.Update;

public class UpdateFireAtEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/reminders/{id}/update-fire-at", (string Id, ISender sender) =>
        {
            var command = new UpdateFireAtCommand(Id);
            return sender.Send(command);
        });
    }
}