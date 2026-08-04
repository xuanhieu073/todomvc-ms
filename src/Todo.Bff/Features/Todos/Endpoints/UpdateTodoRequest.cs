namespace Todo.Bff.Features.Todos.Endpoints
{
    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool isCompleted { get; set; }
        public DateTime DueAt { get; set; }
    }
}
