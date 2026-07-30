using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Reminders.Application.Commands;

public class SnoozeReminderHandler(ReminderApiClient _apiClient) : IRequestHandler<SnoozeReminderCommand, ApiResult>
{
    public async Task<ApiResult> Handle(SnoozeReminderCommand request, CancellationToken cancellationToken)
    {
        return await _apiClient.SnoozeReminder(request.Id, request.snoozeMinutes);
    }
}