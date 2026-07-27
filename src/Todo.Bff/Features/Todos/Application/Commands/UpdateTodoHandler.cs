using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Todos.Application.Commands;

public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, IResult>
{
    private readonly TodoApiClient _apiClient;

    public UpdateTodoHandler(TodoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IResult> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {

        var response = await _apiClient.UpdateTodoAsync(request.Id, request.updateTodoRequest);
        return response.ToHttpResponse();
    }
}