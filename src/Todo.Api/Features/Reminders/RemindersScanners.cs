using System.Text.Json;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders
{
    public class RemindersScanners : BackgroundService
    {
        private readonly ILogger<RemindersScanners> _logger;
        private readonly IMapper _mapper;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly TimeSpan _delayInterval = TimeSpan.FromSeconds(2);

        public RemindersScanners(ILogger<RemindersScanners> logger, IMapper mapper, ServiceBusClient serviceBusClient)
        {
            _logger = logger;
            _mapper = mapper;
            _serviceBusClient = serviceBusClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task.Delay Worker started.");

            DateTime? fireAt = null;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (fireAt == null) { fireAt = DateTime.UtcNow; }
                    var fireAtString = ((DateTime)fireAt).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
                    await DoWorkAsync(stoppingToken, fireAt);
                    fireAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during worker execution.");
                }

                try
                {
                    await Task.Delay(_delayInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("Task.Delay Worker stopped cleanly.");
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken, DateTime? fireAt)
        {
            _logger.LogInformation("Task.Delay function executed at: {Time}", DateTimeOffset.Now);

            var now = DateTime.UtcNow;

            ServiceBusSender sender = _serviceBusClient.CreateSender("queue.1");
            ServiceBusSender sender2 = _serviceBusClient.CreateSender("queue.2");

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
                .Where(x => x.todo.DueAt <= now && x.todo.IsCompleted == false && (x.reminder == null || x.reminder.State == ReminderState.Dismissed))
                // .Select(x => x.todo)
                .ToListAsync();

            if (todosNeedToReminder.Any())
            {
                var newReminders = todosNeedToReminder
                    .Where(x => x.reminder == null)
                    .Select(x => new Reminder { TodoId = x.todo.ID, State = ReminderState.Pending, FiredAt = DateTime.UtcNow, DueAt = x.todo.DueAt })
                    .ToList();
                if (newReminders.Any())
                {
                    await DB.SaveAsync(newReminders);
                    var newPendingReminders = newReminders.Join(todosNeedToReminder, r => r.TodoId, x => x.todo.ID, (r, x) => new PendingReminderDto { Id = r.ID, TodoId = r.TodoId, Title = x.todo.Title }).ToList();
                    string jsonPayload = JsonSerializer.Serialize(newPendingReminders);
                    ServiceBusMessage message = new ServiceBusMessage(jsonPayload);
                    await sender.SendMessageAsync(message, cancellationToken);
                }


                // var dimissedReminders = todosNeedToReminder
                //    .Where(x => x.todo.CompletedAt != null && x.reminder != null && x.reminder.State == ReminderState.Dismissed && x.reminder.DimissAt == null)
                //    .Select(x => x.reminder!)
                //    .ToList();

                // if (dimissedReminders.Any())
                // {
                //     foreach (var reminder in dimissedReminders)
                //     {
                //         reminder.State = ReminderState.Pending;
                //         reminder.FiredAt = DateTime.UtcNow;
                //         reminder.DimissAt = null;
                //     }
                //     await dimissedReminders.SaveAsync();
                //     var newPendingReminders = dimissedReminders.Join(todosNeedToReminder, r => r.TodoId, x => x.todo.ID, (r, x) => new PendingReminderDto { Id = r.ID, TodoId = r.TodoId, Title = x.todo.Title }).ToList();
                //     string jsonPayload = JsonSerializer.Serialize(newPendingReminders);
                //     ServiceBusMessage message = new ServiceBusMessage(jsonPayload);
                //     await sender.SendMessageAsync(message, cancellationToken);
                // }
            }

            var overSnoozedRemindersInfo = await DB.Collection<Reminder>().AsQueryable()
                .GroupJoin(
                    DB.Collection<TodoItem>(),
                    r => r.TodoId,
                    td => td.ID,
                    (reminder, todos) => new { reminder, todos }
                )
                .SelectMany(
                    x => x.todos.DefaultIfEmpty(),
                    (x, todo) => new { x.reminder, todo }
                )
                .Where(r => r.reminder.State == ReminderState.Snoozed && r.reminder.SnoozeUntil <= now)
                .ToListAsync();
            var overSnoozedReminders = overSnoozedRemindersInfo.Select(x => x.reminder);
            if (overSnoozedReminders.Any())
            {
                foreach (var reminder in overSnoozedReminders)
                {
                    reminder.State = ReminderState.Pending;
                    reminder.FiredAt = DateTime.UtcNow;
                }
                await overSnoozedReminders.SaveAsync();

                var newPendingReminders = overSnoozedReminders.Join(overSnoozedRemindersInfo, r => r.TodoId, x => x.todo!.ID, (r, x) => new PendingReminderDto { Id = r.ID, TodoId = r.TodoId, Title = x.todo!.Title }).ToList();
                string jsonPayload = JsonSerializer.Serialize(newPendingReminders);
                ServiceBusMessage message = new ServiceBusMessage(jsonPayload);
                await sender.SendMessageAsync(message, cancellationToken);
            }

            var remindersNeedToDimiss = await DB.Collection<Reminder>()
                .AsQueryable()
                .GroupJoin(
                    DB.Collection<TodoItem>(),
                    r => r.TodoId,
                    td => td.ID,
                    (reminder, todos) => new { reminder, todos }
                )
                .SelectMany(
                    x => x.todos.DefaultIfEmpty(),
                    (x, todo) => new { x.reminder, todo }
                )
                .Where(x => (x.todo == null || x.todo.IsCompleted == true) && x.reminder.DimissAt == null)
                .Select(x => x.reminder)
                .ToListAsync();

            if (remindersNeedToDimiss.Any())
            {
                foreach (var reminder in remindersNeedToDimiss)
                {
                    reminder.State = ReminderState.Dismissed;
                    reminder.DimissAt = DateTime.UtcNow;
                }
                await remindersNeedToDimiss.SaveAsync();
                string jsonPayload = JsonSerializer.Serialize(remindersNeedToDimiss.Select(r => new PendingReminderDto { Id = r.ID }));
                ServiceBusMessage message = new ServiceBusMessage(jsonPayload);
                await sender2.SendMessageAsync(message, cancellationToken);
            }
        }
    }
}
