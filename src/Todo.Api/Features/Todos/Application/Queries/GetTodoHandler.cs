using AutoMapper;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Application.Queries
{
    public class GetTodoHandler : IRequestHandler<GetTodoQuery, TodoDto>
    {
        private readonly IMapper _mapper;
        public GetTodoHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<TodoDto> Handle(GetTodoQuery request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().OneAsync(request.Id);
            return _mapper.Map<TodoDto>(todo);
        }
    }
}
