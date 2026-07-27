using MediatR;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public sealed record ClearCompletedCommand() : IRequest<long>;
}
