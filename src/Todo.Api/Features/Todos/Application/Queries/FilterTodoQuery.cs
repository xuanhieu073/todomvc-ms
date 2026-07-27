using MediatR;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Application.Queries
{
    public sealed record FilterTodoQuery(string filter) : IRequest<List<TodoDto>>;
}
