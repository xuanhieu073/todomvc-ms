using Carter;
using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Todos.Application.Queries;

namespace Todo.Bff.Features.Todos.Endpoints.Get
{
    public class GetTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/bff/todos/{id}", async (string Id, ISender sender) =>
            {
                var command = new GetTodoQuery(Id);
                return await sender.Send(command);
            });
        }
    }
}
