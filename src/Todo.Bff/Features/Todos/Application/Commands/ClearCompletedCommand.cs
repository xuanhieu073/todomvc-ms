using MediatR;

namespace Todo.Bff.Features.Todos.Application.Commands;

public sealed record ClearCompletedCommand() : IRequest<IResult>;