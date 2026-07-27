using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;
using MongoDB.Driver.Linq;
using AutoMapper;

namespace Todo.Api.Features.Todos.Application.Queries
{
    public class FilterTodoHandler : IRequestHandler<FilterTodoQuery, List<TodoDto>>
    {
        private readonly IMapper _mapper;

        public FilterTodoHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<TodoDto>> Handle(FilterTodoQuery request, CancellationToken cancellationToken)
        {
            var query = DB.Queryable<TodoItem>();
            query = request.filter switch
            {
                "active" => query.Where(t => !t.IsCompleted),
                "completed" => query.Where(t => t.IsCompleted),
                _ => query
            };
            var todos = await query.ToListAsync();
            var result = _mapper.Map<List<TodoDto>>(todos);
            return result;
        }
    }
}
