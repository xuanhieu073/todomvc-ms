using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints;

public class UpdateTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/bff/todos/{id}", async (string Id, UpdateTodoRequest updateTodoRequest, ISender sender) =>
        {
            var command = new UpdateTodoCommand(Id, updateTodoRequest);
            return (await sender.Send(command)).ToHttpResponse();
        });
    }

    public sealed record UpdateTodoCommand(string Id, UpdateTodoRequest updateTodoRequest) : IRequest<ApiResult>;

    public class UpdateTodoHandler(TodoApiClient _apiClient) : IRequestHandler<UpdateTodoCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {

            var response = await _apiClient.UpdateTodoAsync(request.Id, request.updateTodoRequest);
            return response;
        }
    }
}