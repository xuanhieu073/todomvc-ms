using Carter;
using MediatR;
using Todo.Bff.Features.Todos.Application.Commands;

namespace Todo.Bff.Features.Todos.Endpoints.Delete;

public class DeleteTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/bff/todos/{id}", async (string Id, ISender sender) =>
        {
            var command = new DeleteTodoCommand(Id);
            return await sender.Send(command);
        });
    }
}