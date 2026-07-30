using Todo.Bff.Features.Reminders.DTOs;

namespace Todo.Bff.Clients;

public class ReminderApiClient(HttpClient httpClient) : ApiClient(httpClient)
{


    public async Task<ApiResult> GetPendingReminders(ReminderState state, CancellationToken cancellationToken = default)
    {
        return state switch
        {
            ReminderState.Pending => await GetAsync<List<PendingReminderDto>>($"/api/reminders?state=pending", cancellationToken),
            ReminderState.Snoozed => await GetAsync<List<PendingReminderDto>>($"/api/reminders?state=snoozed", cancellationToken),
            ReminderState.Dismissed => await GetAsync<List<PendingReminderDto>>($"/api/reminders?state=dismissed", cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    public async Task<ApiResult> UpdateReminderFireAt(string Id, CancellationToken cancellationToken = default)
    {
        return await SendAsync<ReminderDto, string>(HttpMethod.Patch, $"/api/reminders/{Id}/update-fire-at", null, cancellationToken);
    }

    public async Task<ApiResult> SnoozeReminder(string Id, SnoozeReminderReuqest reuqest, CancellationToken cancellationToken = default)
    {
        return await SendAsync<ReminderDto, string>(HttpMethod.Patch, $"/api/reminders/{Id}/snooze", reuqest, cancellationToken);
    }

    public async Task<ApiResult> DimissReminder(string Id, CancellationToken cancellationToken = default)
    {
        return await SendAsync<ReminderDto, string>(HttpMethod.Patch, $"/api/reminders/{Id}/dimiss", null, cancellationToken);
    }
}