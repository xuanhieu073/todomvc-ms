using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints;

public class CreateTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/bff/todos", async (CreateTodoRequest createTodoRequest, ISender sender) =>
        {
            var command = new CreateTodoCommand(createTodoRequest);
            return (await sender.Send(command)).ToHttpResponse();
        });
    }

    public sealed record CreateTodoCommand(CreateTodoRequest createTodoRequest) : IRequest<ApiResult>;

    public class CreateTodoHandler(TodoApiClient _apiClient) : IRequestHandler<CreateTodoCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {

            var response = await _apiClient.CreateTodoAsync(request.createTodoRequest);
            return response;
        }
    }
}