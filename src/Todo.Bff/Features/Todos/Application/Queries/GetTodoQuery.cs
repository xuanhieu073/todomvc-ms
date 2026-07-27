using MediatR;

namespace Todo.Bff.Features.Todos.Application.Queries
{
    public sealed record GetTodoQuery(string Id) : IRequest<IResult>;
}
