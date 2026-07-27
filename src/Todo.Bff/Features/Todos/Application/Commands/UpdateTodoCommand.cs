using MediatR;
using Todo.Bff.Features.Todos.DTOs;

namespace Todo.Bff.Features.Todos.Application.Commands;

public sealed record UpdateTodoCommand(string Id, UpdateTodoRequest updateTodoRequest) : IRequest<IResult>;