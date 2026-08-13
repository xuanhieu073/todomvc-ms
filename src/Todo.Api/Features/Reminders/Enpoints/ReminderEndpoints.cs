using Carter;
using Todo.Api.Common;

namespace Todo.Api.Features.Reminders.Enpoints;

public partial class ReminderEndpoints : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    var apiGroup = app.MapGroup("/api/reminders").RequireAuthorization().AddEndpointFilter<UserBindingFilter>();
    AddPendingReminderRoute(apiGroup);
    AddUpcompingRoute(apiGroup);
    AddDimissedReminderRoute(apiGroup);
    AddSnoozeReminderRoute(apiGroup);
    AddDimissReminderRoute(apiGroup);
  }
}