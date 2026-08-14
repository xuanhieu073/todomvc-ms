using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Reminders.Enpoints;

public class PendingReminderEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/bff/reminders", async (string state, ISender sender) =>
    {
      var query = new PendingReminderQuery(state);
      var response = await sender.Send(query);
      return response.ToHttpResponse();
    });
  }

  public sealed record PendingReminderQuery(string State) : IRequest<ApiResult>;

  public class PendingReminderHandler(ReminderApiClient apiClient) : IRequestHandler<PendingReminderQuery, ApiResult>
  {
    Task<ApiResult> IRequestHandler<PendingReminderQuery, ApiResult>.Handle(PendingReminderQuery request,
      CancellationToken cancellationToken)
    {
      var reminderState = request.State switch
      {
        "pending" => ReminderState.Pending,
        "dimissed" => ReminderState.Dismissed,
        "snoozed" => ReminderState.Snoozed,
        _ => ReminderState.Pending
      };
      return apiClient.GetPendingReminders(reminderState, cancellationToken);
    }
  }
}