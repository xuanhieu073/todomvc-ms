using Todo.Bff.Features.Reminders;
using Todo.Bff.Features.Reminders.Enpoints;

namespace Todo.Bff.Clients;

public class ReminderApiClient(HttpClient httpClient) : ApiClient(httpClient)
{
    public async Task<ApiResult> SnoozeReminder(string Id, SnoozeReminderReuqest reuqest,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<ReminderDto>(HttpMethod.Patch, $"/api/reminders/{Id}/snooze", reuqest,
            cancellationToken);
    }

    public async Task<ApiResult> DimissReminder(string Id, CancellationToken cancellationToken = default)
    {
        return await SendAsync<ReminderDto>(HttpMethod.Patch, $"/api/reminders/{Id}/dimiss", null, cancellationToken);
    }
}