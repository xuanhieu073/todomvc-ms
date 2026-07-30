using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Reminders.Application.Queries;
using Todo.Bff.Features.Reminders.DTOs;

public class RemindersStreamHandler(TodoApiClient _apiClient) : IRequestHandler<RemindersStreamQuery, List<ReminderDto>>
{
    public async Task<List<ReminderDto>> Handle(RemindersStreamQuery request, CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetPendingReminders(request.state, cancellationToken);
        return response.StatusCode switch
        {
            200 => ((ApiSucessResult<List<ReminderDto>>)response).Data ?? new List<ReminderDto>(),
            _ => throw new Exception($"Error fetching reminders: {response.StatusCode} - {response.ErrorMessage}")
        };
    }
}