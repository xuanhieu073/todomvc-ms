using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Reminders.Application.Commands;

public class UpdateFireAtHandler(ReminderApiClient _apiClient) : IRequestHandler<UpdateFireAtCommand, ApiResult>
{
    public async Task<ApiResult> Handle(UpdateFireAtCommand request, CancellationToken cancellationToken)
    {
        var response = await _apiClient.UpdateReminderFireAt(request.Id, cancellationToken);
        return response;
    }
}