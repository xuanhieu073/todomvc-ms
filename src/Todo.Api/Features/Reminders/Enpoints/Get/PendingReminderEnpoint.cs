using Carter;
using MediatR;
using Todo.Api.Features.Reminders.Application.Queries;

namespace Todo.Api.Features.Reminders.Endpoints.Get;

public class PendingReminderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reminders", (string state, ISender sender) =>
        {
            var query = new PendingReminderQuery(state);
            return sender.Send(query);
        });
    }
}