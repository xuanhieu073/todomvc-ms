using MediatR;
using Todo.Bff.Features.Todos.DTOs;

namespace Todo.Bff.Features.Todos.Application.Commands;

public sealed record CreateTodoCommand(CreateTodoRequest createTodoRequest) : IRequest<IResult>;