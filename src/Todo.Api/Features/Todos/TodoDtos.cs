namespace Todo.Api.Features.Todos
{
    public class TodoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime DueAt { get; set; }
    }
}
