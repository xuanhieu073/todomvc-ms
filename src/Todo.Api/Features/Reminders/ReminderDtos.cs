using MongoDB.Entities;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Reminders
{
    public class ReminderDto
    {
        public string Id { get; set; }
        public string TodoId { get; set; }
        public DateTime DueAt { get; set; }
        public ReminderState State { get; set; }
        public DateTime? SnoozeUntil { get; set; }
        public DateTime? FiredAt { get; set; }
    }

    public class ReminderTodoDto
    {
        public Reminder Reminder { get; set; }
        public TodoItem Todo { get; set; }
    }

    public class PendingReminderDto
    {
        public string Id { get; set; }
        public string TodoId { get; set; }
        public string Title { get; set; }
        public ReminderState State { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime FireAt { get; set; }
    }
}
