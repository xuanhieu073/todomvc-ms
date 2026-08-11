using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints
{
    public class FilterTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/bff/todos",
                async ([AsParameters] FilterTodoQuery query, ISender sender) =>
                (await sender.Send(query)).ToHttpResponse());
        }
    }

    public sealed record FilterTodoQuery(string Filter) : IRequest<ApiResult>;

    public class FilterTodoHandler(TodoApiClient apiClient) : IRequestHandler<FilterTodoQuery, ApiResult>
    {
        public async Task<ApiResult> Handle(FilterTodoQuery request, CancellationToken cancellationToken)
        {
            var response = await apiClient.GetTodosAsync(request.Filter, cancellationToken);
            return response;
        }
    }
}
