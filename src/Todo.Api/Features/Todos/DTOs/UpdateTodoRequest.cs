namespace Todo.Api.Features.Todos.DTOs
{
    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool isCompleted { get; set; }
    }
}
