using Carter;
using MediatR;

namespace Todo.Api.Features.Reminders.Endpoints.Get;

public class UpcomingReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reminders/upcoming", async (string within, ISender sender) =>
        {
            var query = new UpcomingReminderQuery(within);
            return await sender.Send(query);
        });
    }
}