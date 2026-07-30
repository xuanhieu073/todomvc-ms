using Carter;
using Common;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Endpoints.Get;

public class QueryTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/todos/query", async (QueryRequest req) =>
        {
            var result = await QueryRequest.RunQuery<TodoItem>(req);
            return Results.Ok(result);
        });
    }
}