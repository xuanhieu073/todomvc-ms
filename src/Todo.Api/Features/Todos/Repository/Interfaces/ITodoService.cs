using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Todos.Repository.Interfaces
{
    public interface ITodoService
    {
        Task<List<TodoDto>> Filter(string status);
        Task<TodoDto?> GetTodoById(string Id);
        Task<TodoDto> CreateTodo(CreateToDoRequest createTodo);
        Task<TodoDto?> UpdateTodo(string Id, UpdateTodoRequest updateTodo);
        Task<TodoDto?> ToggleIsCompleted(string Id);
        Task<bool?> DeleteTodo(string Id);
        Task<long?> ClearCompleted();
    }
}
