using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints;

public class ClearCompleted : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/bff/todos/completed", async (ISender sender) =>
        {
            var command = new ClearCompletedCommand();
            return (await sender.Send(command)).ToHttpResponse();
        });
    }

    public sealed record ClearCompletedCommand : IRequest<ApiResult>;

    public class ClearCompletedHandler(TodoApiClient apiClient) : IRequestHandler<ClearCompletedCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(ClearCompletedCommand request, CancellationToken cancellationToken)
        {
            var response = await apiClient.ClearCompleted(cancellationToken);
            return response;
        }
    }
}