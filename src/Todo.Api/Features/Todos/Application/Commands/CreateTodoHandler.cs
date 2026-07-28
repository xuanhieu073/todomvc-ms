using AutoMapper;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, TodoDto>
    {
        private readonly IMapper _mapper;
        private readonly IValidator<CreateTodoCommand> _validator;

        public CreateTodoHandler(IMapper mapper, IValidator<CreateTodoCommand> validator)
        {
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<TodoDto> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(command);
            var newTodo = _mapper.Map<CreateTodoRequest, TodoItem>(command.createTodoRequest);
            newTodo.CreatedAt = DateTime.Now;
            await newTodo.SaveAsync();
            return _mapper.Map<TodoDto>(newTodo);
        }
    }
}
