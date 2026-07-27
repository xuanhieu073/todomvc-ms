using Carter;
using MediatR;
using Todo.Api.Features.Todos.Application.Commands;

namespace Todo.Api.Features.Todos.Endpoints.Delete
{
    public class DeleteTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/todos/{id}", async (string Id, ISender sender) =>
            {
                var command = new DeleteTodoCommand(Id);
                var result = await sender.Send(command);
                return result switch
                {
                    null => Results.NotFound("Invalid Id"),
                    false => Results.InternalServerError(),
                    true => Results.Ok(),
                };
            });
        }
    }
}
