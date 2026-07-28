using AutoMapper;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, TodoDto?>
    {
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateTodoCommand> _validator;

        public UpdateTodoHandler(IMapper mapper, IValidator<UpdateTodoCommand> validator)
        {
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<TodoDto?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(request);
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
