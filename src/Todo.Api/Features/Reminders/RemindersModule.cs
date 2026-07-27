using Carter;

namespace Todo.Api.Features.Reminders
{
    public class RemindersModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiGroup = app.MapGroup("/api/reminders");

            apiGroup.MapGet("", () => "");
            apiGroup.MapGet("/upcoming", () => "");
            apiGroup.MapPatch("/{id}/snooze", () => "");
            apiGroup.MapPatch("/{id}/dimiss", () => "");
        }
    }
}
