using MongoDB.Entities;
using Todo.Api.Features.Reminders;
using Todo.Api.Features.Todos;

namespace Todo.Api.Common;

public static class MongoIndexConfig
{
    public static async Task InitializeIndexesAsync()
    {
        // 1. TodoItem Indexes
        await DB.Index<TodoItem>()
            .Key(t => t.IsCompleted, KeyType.Ascending)
            .Key(t => t.DueAt, KeyType.Ascending)
            .CreateAsync();

        // 2. Reminder Indexes
        await DB.Index<Reminder>()
            .Key(r => r.TodoId, KeyType.Ascending)
            .CreateAsync();

        await DB.Index<Reminder>()
            .Key(r => r.State, KeyType.Ascending)
            .CreateAsync();
    }
}