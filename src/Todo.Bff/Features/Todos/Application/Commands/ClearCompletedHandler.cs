using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Application.Commands;

public class ClearCompletedHandler(TodoApiClient _apiClient) : IRequestHandler<ClearCompletedCommand, IResult>
{
    public async Task<IResult> Handle(ClearCompletedCommand request, CancellationToken cancellationToken)
    {
        var response = await _apiClient.ClearCompleted();
        return response.ToHttpResponse();
    }
}