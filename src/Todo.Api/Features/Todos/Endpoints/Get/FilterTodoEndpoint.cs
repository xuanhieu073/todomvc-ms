using Carter;
using Carter.Request;
using MediatR;
using Todo.Api.Features.Todos.Application.Queries;

namespace Todo.Api.Features.Todos.Endpoints.Get
{
    public class FilterTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/todos", async (HttpRequest req, ISender sender) => {
                var filter = req.Query.As<string>("filter");
                var command = new FilterTodoQuery(filter);
                var result = await sender.Send(command);
                return result;
            });
        }
    }
}
