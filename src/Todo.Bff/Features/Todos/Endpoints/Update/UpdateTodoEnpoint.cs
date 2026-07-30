using Carter;
using MediatR;
using Todo.Bff.Features.Todos.Application.Commands;
using Todo.Bff.Features.Todos.DTOs;

namespace Todo.Bff.Features.Todos.Endpoints.Update;

public class UpdateTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/bff/todos/{id}", async (string Id, UpdateTodoRequest updateTodoRequest, ISender sender) =>
        {
            var command = new UpdateTodoCommand(Id, updateTodoRequest);
            return await sender.Send(command);
        });
    }
}