using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints
{
    public class GetTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/bff/todos/{id}",
                async ([AsParameters] GetTodoQuery query, ISender sender) =>
                (await sender.Send(query)).ToHttpResponse());
        }
    }

    public sealed record GetTodoQuery(string Id) : IRequest<ApiResult>;

    public class GetTodoHandler(TodoApiClient apiClient) : IRequestHandler<GetTodoQuery, ApiResult>
    {
        public async Task<ApiResult> Handle(GetTodoQuery request, CancellationToken cancellationToken)
        {
            var response = await apiClient.GetTodoAsync(request.Id, cancellationToken);
            return response;
        }
    }
}
