using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public class ClearCompletedHandler : IRequestHandler<ClearCompletedCommand, long>
    {
        public async Task<long> Handle(ClearCompletedCommand request, CancellationToken cancellationToken)
        {
            var deletedResult = await DB.DeleteAsync<TodoItem>(todo => todo.IsCompleted == true);
            return deletedResult.IsAcknowledged switch
            {
                true => deletedResult.DeletedCount,
                _ => 0,
            };
        }
    }
}
