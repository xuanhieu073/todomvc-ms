using MediatR;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Application.Queries
{
    public sealed record GetTodoQuery(string Id) : IRequest<TodoDto>;
}
