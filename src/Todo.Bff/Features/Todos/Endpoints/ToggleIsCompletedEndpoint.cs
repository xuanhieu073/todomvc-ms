using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints;

public class ToggleIsCompletedEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/bff/todos/{id}/toggle", async (string Id, ISender sender) =>
        {
            var command = new ToggleIsCompletedCommand(Id);
            var response = await sender.Send(command);
            return response.ToHttpResponse();
        });
    }

    public sealed record ToggleIsCompletedCommand(string Id) : IRequest<ApiResult>;

    public class ToggleIsCompletedHandler(TodoApiClient _apiClient) : IRequestHandler<ToggleIsCompletedCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(ToggleIsCompletedCommand request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.ToggleIsCompleted(request.Id);
            return response;
        }
    }
}