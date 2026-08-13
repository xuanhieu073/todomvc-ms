using AutoMapper;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public partial class TodoEndpoint
    {
        public void AddGetTodoRoute(IEndpointRouteBuilder app)
        {
            app.MapGet("/{id}", async ([AsParameters] GetTodoQuery query, ISender sender) =>
            await sender.Send(query));
        }
    }

    public sealed record GetTodoQuery(string Id) : UserBoundRequest, IRequest<TodoResponse>;

    public class GetTodoHandler(IMapper mapper) : IRequestHandler<GetTodoQuery, TodoResponse>
    {
        public async Task<TodoResponse> Handle(GetTodoQuery request, CancellationToken cancellationToken)
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
                return mapper.Map<TodoResponse>(todo);
            }
        }
    }
}
