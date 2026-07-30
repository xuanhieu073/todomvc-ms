using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Reminders.Application.Queries;
using Todo.Bff.Features.Reminders.DTOs;

public class RemindersStreamHandler(ReminderApiClient _apiClient) : IRequestHandler<RemindersStreamQuery, List<PendingReminderDto>>
{
    public async Task<List<PendingReminderDto>> Handle(RemindersStreamQuery request, CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetPendingReminders(request.state, cancellationToken);
        return response.StatusCode switch
        {
            200 => ((ApiSucessResult<List<PendingReminderDto>>)response).Data ?? new List<PendingReminderDto>(),
            _ => throw new Exception($"Error fetching reminders: {response.StatusCode} - {response.ErrorMessage}")
        };
    }
}