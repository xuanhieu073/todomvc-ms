using Todo.Bff.Features.Reminders;
using Todo.Bff.Features.Reminders.Enpoints;

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

    public async Task<ApiResult> GetUpcomingReminders(string within, string fireAt, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<PendingReminderDto>>($"/api/reminders/upcoming?within={within}&fireAt={fireAt}", cancellationToken);
    }

    public async Task<ApiResult> GetDimissedReminders(string dimissedFrom, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<ReminderDto>>($"/api/reminders/dimissed?from={dimissedFrom}", cancellationToken);
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