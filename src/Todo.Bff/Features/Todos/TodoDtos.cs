namespace Todo.Bff.Features.Todos
{
    public class TodoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool isCompleted { get; set; }
        public DateTime DueAt { get; set; }
    }
    public class TodoPropertiesError
    {
        public string propertyName { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;
    }
}
