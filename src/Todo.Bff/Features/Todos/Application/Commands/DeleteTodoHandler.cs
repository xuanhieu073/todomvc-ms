using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Application.Commands;

public class DeleteTodoHandler(TodoApiClient _apiClient) : IRequestHandler<DeleteTodoCommand, IResult>
{
    public async Task<IResult> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var response = await _apiClient.DelteTodo(request.Id);
        return response.ToHttpResponse();
    }
}