using MediatR;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public sealed record ToggleIsCompletedCommand(string Id) : IRequest<TodoDto?>;
}
