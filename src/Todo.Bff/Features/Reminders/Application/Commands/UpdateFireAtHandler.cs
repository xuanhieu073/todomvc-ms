using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Reminders.Application.Commands;

public class UpdateFireAtHandler : IRequestHandler<UpdateFireAtCommand, ApiResult>
{
    private readonly TodoApiClient _apiClient;

    public UpdateFireAtHandler(TodoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResult> Handle(UpdateFireAtCommand request, CancellationToken cancellationToken)
    {
        var response = await _apiClient.UpdateReminderFireAt(request.Id, cancellationToken);
        return response;
    }
}