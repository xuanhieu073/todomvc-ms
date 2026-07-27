using Carter;
using MediatR;
using Todo.Bff.Features.Todos.Application.Commands;
using Todo.Bff.Features.Todos.DTOs;

namespace Todo.Bff.Features.Todos.Endpoints.Create;

public class CreateTodoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/bff/todos", async (CreateTodoRequest createTodoRequest, ISender sender) =>
        {
            var command = new CreateTodoCommand(createTodoRequest);
            return await sender.Send(command);
        });
    }
}