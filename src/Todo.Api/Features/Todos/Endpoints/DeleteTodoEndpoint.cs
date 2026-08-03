using Carter;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public class DeleteTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/todos/{id}", async ([AsParameters] DeleteTodoCommand command, ISender sender) =>
                await sender.Send(command));
        }
    }

    public sealed record DeleteTodoCommand(string Id) : IRequest<bool?>;

    public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, bool?>
    {
        public async Task<bool?> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().OneAsync(request.Id);
            if (todo == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }
            else
            {
                var result = await DB.DeleteAsync<TodoItem>(request.Id);
                return result.IsAcknowledged;
            }
        }
    }
}
