using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Reminders.Enpoints;

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
public sealed record SnoozeReminderCommand(string Id, SnoozeReminderReuqest snoozeMinutes) : IRequest<ApiResult>;

public class SnoozeReminderHandler(ReminderApiClient _apiClient) : IRequestHandler<SnoozeReminderCommand, ApiResult>
{
    public async Task<ApiResult> Handle(SnoozeReminderCommand request, CancellationToken cancellationToken)
    {
        return await _apiClient.SnoozeReminder(request.Id, request.snoozeMinutes);
    }
}