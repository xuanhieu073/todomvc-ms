using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Reminders.DTOs;

namespace Todo.Bff.Features.Reminders.Application.Commands;

public class DimissReminderHandler(ReminderApiClient _apiClient) : IRequestHandler<DimissReminderCommand, ApiResult>
{
    public Task<ApiResult> Handle(DimissReminderCommand request, CancellationToken cancellationToken)
    {
        return _apiClient.DimissReminder(request.Id);
    }
}