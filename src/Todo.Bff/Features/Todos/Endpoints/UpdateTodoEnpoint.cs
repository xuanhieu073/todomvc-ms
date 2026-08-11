using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints;

public class UpdateTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/bff/todos/{id}", async (string id, UpdateTodoRequest updateTodoRequest, ISender sender) =>
        {
            var command = new UpdateTodoCommand(id, updateTodoRequest);
            return (await sender.Send(command)).ToHttpResponse();
        });
    }

    public sealed record UpdateTodoCommand(string Id, UpdateTodoRequest UpdateTodoRequest) : IRequest<ApiResult>;

    public class UpdateTodoHandler(TodoApiClient apiClient) : IRequestHandler<UpdateTodoCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            var response = await apiClient.UpdateTodoAsync(request.Id, request.UpdateTodoRequest, cancellationToken);
            return response;
        }
    }
}