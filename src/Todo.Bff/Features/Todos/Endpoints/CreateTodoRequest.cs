namespace Todo.Bff.Features.Todos.Endpoints
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public DateTime DueAt { get; set; }
    }
}
