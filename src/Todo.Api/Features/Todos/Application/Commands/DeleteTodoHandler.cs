using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, bool?>
    {
        public async Task<bool?> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().OneAsync(request.Id);
            if (todo == null)
            {
                return null;
            }
            else
            {
                var result = await DB.DeleteAsync<TodoItem>(request.Id);
                return result.IsAcknowledged;
            }
        }
    }
}
