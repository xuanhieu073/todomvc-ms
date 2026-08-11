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

    public sealed record CreateTodoCommand(CreateTodoRequest CreateTodoRequest) : IRequest<ApiResult>;

    public class CreateTodoHandler(TodoApiClient apiClient) : IRequestHandler<CreateTodoCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var response = await apiClient.CreateTodoAsync(request.CreateTodoRequest, cancellationToken);
            return response;
        }
    }
}