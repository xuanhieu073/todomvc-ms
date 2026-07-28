using Carter;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Todo.Api.Features.Todos.Application.Commands;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Endpoints.Update
{
    public class UpdateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/todos/{id}", async (string Id, UpdateTodoRequest updateTodoRequest, ISender sender) =>
            {
                var command = new UpdateTodoCommand(Id, updateTodoRequest);
                var result = await sender.Send(command);
                return result switch
                {
                    null => Results.NotFound("Invalid Id"),
                    _ => Results.Ok(result)
                };
            });
        }
    }
}
