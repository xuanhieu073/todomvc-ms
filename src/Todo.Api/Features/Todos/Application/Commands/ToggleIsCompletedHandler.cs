using AutoMapper;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public class ToggleIsCompletedHandler : IRequestHandler<ToggleIsCompletedCommand, TodoDto?>
    {
        private readonly IMapper _mapper;
        
        public ToggleIsCompletedHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<TodoDto?> Handle(ToggleIsCompletedCommand request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().OneAsync(request.Id);
            if (todo == null)
            {
                return null;
            }
            else
            {
                todo.IsCompleted = !todo.IsCompleted;
                await todo.SaveAsync();
                return _mapper.Map<TodoDto>(todo);
            }
        }
    }
}
