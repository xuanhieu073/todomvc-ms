using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Application.Commands;

public class ToggleIsCompletedHandler(TodoApiClient _apiClient) : IRequestHandler<ToggleIsCompletedCommand, ApiResult>
{
    public async Task<ApiResult> Handle(ToggleIsCompletedCommand request, CancellationToken cancellationToken)
    {
        var response = await _apiClient.ToggleIsCompleted(request.Id);
        return response;
    }
}