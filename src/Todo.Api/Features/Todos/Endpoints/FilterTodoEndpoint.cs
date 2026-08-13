using AutoMapper;
using MediatR;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public partial class TodoEndpoint
    {
        public void AddFilterTodoRoute(IEndpointRouteBuilder app)
        {
            app.MapGet("",
                async ([AsParameters] FilterTodoQuery query, ISender sender) =>
                await sender.Send(query));
        }
    }

    public sealed record FilterTodoQuery(string Filter)
        : UserBoundRequest, IRequest<List<TodoResponse>>;

    public class FilterTodoHandler(IMapper mapper) : IRequestHandler<FilterTodoQuery, List<TodoResponse>>
    {
        public async Task<List<TodoResponse>> Handle(FilterTodoQuery request, CancellationToken cancellationToken)
        {
            var query = DB.Queryable<TodoItem>().Where(t => t.OwnerId == request.UserId);
            query = request.Filter switch
            {
                "active" => query.Where(t => !t.IsCompleted),
                "completed" => query.Where(t => t.IsCompleted),
                _ => query
            };
            var todos = await query.ToListAsync(cancellationToken: cancellationToken);
            var result = mapper.Map<List<TodoResponse>>(todos);
            return result;
        }
    }
}
