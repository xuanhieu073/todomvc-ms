using MongoDB.Entities;

namespace Todo.Api.Entities
{
    public class TodoItem : Entity   // ID string do MongoDB.Entities sinh
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
