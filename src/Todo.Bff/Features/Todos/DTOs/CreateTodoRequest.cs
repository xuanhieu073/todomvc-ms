namespace Todo.Bff.Features.Todos.DTOs
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public DateTime DueAt { get; set; }
    }
}
