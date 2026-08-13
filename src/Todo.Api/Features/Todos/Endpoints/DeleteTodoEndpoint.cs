using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints;

public partial class TodoEndpoint
{
    public void AddDeleteTodoRoute(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id}", async ([AsParameters] DeleteTodoCommand command, ISender sender) =>
        await sender.Send(command)).RequireAuthorization();
    }
}

public sealed record DeleteTodoCommand(string Id) : UserBoundRequest, IRequest<bool?>;

public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, bool?>
{
    public async Task<bool?> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await DB.Find<TodoItem>().Match(t => t.ID == request.Id && t.OwnerId == request.UserId)
            .ExecuteFirstAsync(cancellationToken);
        if (todo == null)
        {
            var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
            List<ValidationError> errors = [error];
            throw new NotFoundException(errors);
        }
        else
        {
            var result = await DB.DeleteAsync<TodoItem>(request.Id, cancellation: cancellationToken);
            return result.IsAcknowledged;
        }
    }
}
