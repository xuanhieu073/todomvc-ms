namespace Todo.Api.Features.Todos.DTOs
{
    public class TodoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool isCompleted { get; set; }
    }
}
