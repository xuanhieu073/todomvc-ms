using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Reminders
{
    public class ReminderRepository
    {
        public void createIfNotExists()
        {

        }

        public void changeOverSnoozedToPeding()
        {

        }

        public async Task clearGarbageReminder()
        {
            var remindersNeedToRemove = await DB.Collection<Reminder>()
                .AsQueryable()
                .GroupJoin(
                    DB.Collection<TodoItem>(),
                    r => r.TodoId,
                    td => td.ID,
                    (reminder, todo) => new { reminder, todo }
                )
                .Where(x => x.todo == null)
                .ToListAsync();
            var reminderIdsNeedToRemove = remindersNeedToRemove.Select(x => x.reminder.ID);
            await DB.DeleteAsync<Reminder>(reminderIdsNeedToRemove);
        }
    }
}
