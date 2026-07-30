namespace Todo.Api.Features.Todos.DTOs
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = "";
        public DateTime? DueAt { get; set; }
    }
}
