using MediatR;

namespace Todo.Bff.Features.Todos.Application.Commands;

public sealed record DeleteTodoCommand(string Id) : IRequest<IResult>;