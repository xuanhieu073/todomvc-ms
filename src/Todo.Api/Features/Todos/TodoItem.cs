using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos
{
    public class TodoItem : Entity // ID string do MongoDB.Entities sinh
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; }

        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
