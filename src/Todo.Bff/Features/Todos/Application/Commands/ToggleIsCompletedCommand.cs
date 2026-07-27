using MediatR;

namespace Todo.Bff.Features.Todos.Application.Commands;

public sealed record ToggleIsCompletedCommand(string Id) : IRequest<IResult>;