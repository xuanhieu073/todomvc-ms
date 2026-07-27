using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Application.Commands;

public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, IResult>
{
    private readonly TodoApiClient _apiClient;

    public CreateTodoHandler(TodoApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IResult> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {

        var response = await _apiClient.CreateTodoAsync(request.createTodoRequest);
        return response.ToHttpResponse();
    }
}