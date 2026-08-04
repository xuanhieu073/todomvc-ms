using Carter;
using Carter.Request;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Endpoints
{
    public class FilterTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/bff/todos", async ([AsParameters] FilterTodoQuery query, ISender sender) =>
            {
                return (await sender.Send(query)).ToHttpResponse();
            });
        }
    }

    public sealed record FilterTodoQuery(string filter) : IRequest<ApiResult>;

    public class FilterTodoHandler(TodoApiClient _apiClient) : IRequestHandler<FilterTodoQuery, ApiResult>
    {
        public async Task<ApiResult> Handle(FilterTodoQuery request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.GetTodosAsync(request.filter);
            return response;
        }
    }
}
