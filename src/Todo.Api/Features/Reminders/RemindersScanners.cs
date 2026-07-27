using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Reminders
{
    public class RemindersScanners : BackgroundService
    {
        private readonly ILogger<RemindersScanners> _logger;
        private readonly IMapper _mapper;
        private readonly TimeSpan _delayInterval = TimeSpan.FromSeconds(1);

        public RemindersScanners(ILogger<RemindersScanners> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task.Delay Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during worker execution.");
                }
            }

            _logger.LogInformation("Task.Delay Worker stopped cleanly.");
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task.Delay function executed at: {Time}", DateTimeOffset.Now);

            var now = DateTime.Now;

            var todosNeedToReminder = await DB.Collection<TodoItem>()
                .AsQueryable()
                .GroupJoin(
                    DB.Collection<Reminder>(),
                    todo => todo.ID,
                    reminder => reminder.TodoId,
                    (todo, reminders) => new { todo, reminders }
                )
                .SelectMany(
                    x => x.reminders.DefaultIfEmpty(),
                    (x, reminder) => new { x.todo, reminder }
                )
                .Where(x => x.todo.DueAt <= now && x.todo.IsCompleted == false && x.reminder == null)
                .Select(x => x.todo)
                .ToListAsync();

            var newReminders = todosNeedToReminder.Select(td => new Reminder { TodoId = td.ID, State = ReminderState.Pending });
            await DB.SaveAsync(newReminders);

            var overSnoozedReminder = await DB.Collection<Reminder>().AsQueryable().Where(r => r.State == ReminderState.Snoozed && r.SnoozeUntil <= now).ToListAsync();
            overSnoozedReminder.Select(r => _mapper.Map<Reminder, Reminder>(new Reminder { State = ReminderState.Pending }));

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

            await Task.Delay(_delayInterval, cancellationToken);
        }
    }
}
