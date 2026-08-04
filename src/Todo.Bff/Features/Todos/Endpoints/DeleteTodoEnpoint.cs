using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints;

public class DeleteTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/bff/todos/{id}", async (string Id, ISender sender) =>
        {
            var command = new DeleteTodoCommand(Id);
            return (await sender.Send(command)).ToHttpResponse();
        });
    }

    public sealed record DeleteTodoCommand(string Id) : IRequest<ApiResult>;

    public class DeleteTodoHandler(TodoApiClient _apiClient) : IRequestHandler<DeleteTodoCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.DelteTodo(request.Id);
            return response;
        }
    }
}