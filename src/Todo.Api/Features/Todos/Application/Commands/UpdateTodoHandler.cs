using AutoMapper;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, TodoDto?>
    {
        private readonly IMapper _mapper;

        public UpdateTodoHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<TodoDto?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().OneAsync(request.Id);
            if (todo == null)
            {
                return null;
            }
            else
            {
                _mapper.Map(request.updateTodoRequest, todo);
                await todo.SaveAsync();
                return _mapper.Map<TodoDto>(todo);
            }
        }
    }
}
