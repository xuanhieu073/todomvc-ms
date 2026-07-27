using Carter;
using Carter.Request;
using MediatR;
using Todo.Bff.Features.Todos.Application.Queries;

namespace Todo.Bff.Features.Todos.Endpoints.Get
{
    public class FilterTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/bff/todos", async (HttpRequest req, ISender sender) => {
                var filter = req.Query.As<string>("filter");
                var query = new FilterTodoQuery(filter);
                return await sender.Send(query);
            });
        }
    }
}
