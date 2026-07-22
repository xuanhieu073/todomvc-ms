using Todo.Api.DTOs;
using Todo.Api.Entities;

namespace Todo.Api.Repository.Interfaces
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
