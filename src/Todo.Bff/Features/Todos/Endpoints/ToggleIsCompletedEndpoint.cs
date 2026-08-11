using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints;

public class ToggleIsCompletedEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/bff/todos/{id}/toggle", async (string id, ISender sender) =>
        {
            var command = new ToggleIsCompletedCommand(id);
            var response = await sender.Send(command);
            return response.ToHttpResponse();
        });
    }

    public sealed record ToggleIsCompletedCommand(string Id) : IRequest<ApiResult>;

    public class ToggleIsCompletedHandler(TodoApiClient apiClient)
        : IRequestHandler<ToggleIsCompletedCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(ToggleIsCompletedCommand request, CancellationToken cancellationToken)
        {
            var response = await apiClient.ToggleIsCompleted(request.Id, cancellationToken);
            return response;
        }
    }
}