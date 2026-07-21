namespace Todo.Api.DTOs
{
    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool isCompleted { get; set; }
    }
}
