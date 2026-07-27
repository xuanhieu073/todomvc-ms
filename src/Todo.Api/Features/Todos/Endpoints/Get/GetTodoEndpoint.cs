using Carter;
using Carter.Request;
using MediatR;
using Todo.Api.Features.Todos.Application.Queries;

namespace Todo.Api.Features.Todos.Endpoints.Get
{
    public class GetTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/todos/{id}", async (string Id, ISender sender) =>
            {
                var command = new GetTodoQuery(Id);
                var result = await sender.Send(command);
                if (result == null)
                {
                    return Results.NotFound("Invalid Id");
                }
                else
                {
                    return Results.Ok(result);
                }
            });
        }
    }
}
