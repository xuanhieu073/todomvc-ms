using MediatR;

namespace Todo.Api.Features.Todos.Application.Commands
{
    public sealed record DeleteTodoCommand(string Id): IRequest<bool?>;
}
