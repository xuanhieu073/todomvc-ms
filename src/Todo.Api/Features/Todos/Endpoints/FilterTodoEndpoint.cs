using AutoMapper;
using Carter;
using MediatR;
using MongoDB.Entities;
using MongoDB.Driver.Linq;

namespace Todo.Api.Features.Todos.Endpoints
{
    public class FilterTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/todos", async ([AsParameters] FilterTodoQuery query, ISender sender) =>
            await sender.Send(query));
        }
    }

    public sealed record FilterTodoQuery(string Filter) : IRequest<List<TodoResponse>>;

    public class FilterTodoHandler(IMapper mapper) : IRequestHandler<FilterTodoQuery, List<TodoResponse>>
    {
        public async Task<List<TodoResponse>> Handle(FilterTodoQuery request, CancellationToken cancellationToken)
        {
            var query = DB.Queryable<TodoItem>();
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
