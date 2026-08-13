using System.Text.Json;
using Azure.Messaging.ServiceBus;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders
{
    public class RemindersScanners(ILogger<RemindersScanners> logger, ServiceBusClient serviceBusClient)
        : BackgroundService
    {
        private readonly TimeSpan _delayInterval = TimeSpan.FromSeconds(2);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Task.Delay Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during worker execution.");
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

            logger.LogInformation("Task.Delay Worker stopped cleanly.");
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Task.Delay function executed at: {Time}", DateTimeOffset.Now);

            var now = DateTime.UtcNow;

            string topicName = "topic.1";
            ServiceBusSender sender = serviceBusClient.CreateSender(topicName);

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
                .Where(x => x.todo.DueAt <= now && x.todo.IsCompleted == false &&
                            (x.reminder == null || x.reminder.State == ReminderState.Dismissed))
                .ToListAsync(cancellationToken: cancellationToken);

            if (todosNeedToReminder.Any())
            {
                var newReminders = todosNeedToReminder
                    .Where(x => x.reminder == null)
                    .Select(x => new Reminder
                    {
                        TodoId = x.todo.ID, State = ReminderState.Pending, FiredAt = DateTime.UtcNow,
                        DueAt = x.todo.DueAt
                    })
                    .ToList();
                if (newReminders.Any())
                {
                    await DB.SaveAsync(newReminders, cancellation: cancellationToken);
                    var newPendingReminders = newReminders.Join(todosNeedToReminder, r => r.TodoId, x => x.todo.ID,
                            (r, x) => new PendingReminderDto { OwnerId = x.todo.OwnerId, Id = r.ID, TodoId = r.TodoId, Title = x.todo.Title })
                        .ToList();
                    string jsonPayload = JsonSerializer.Serialize(newPendingReminders);
                    ServiceBusMessage notificationMessage = new ServiceBusMessage(jsonPayload);
                    notificationMessage.ApplicationProperties.Add("IsNotification", true);
                    notificationMessage.ApplicationProperties.Add("IsEmail", true);
                    notificationMessage.ApplicationProperties.Add("NotificationType", "Add");
                    await sender.SendMessageAsync(notificationMessage, cancellationToken);
                }
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
                .ToListAsync(cancellationToken: cancellationToken);
            var overSnoozedReminders = overSnoozedRemindersInfo.Select(x => x.reminder);
            var snoozedReminders = overSnoozedReminders as Reminder[] ?? overSnoozedReminders.ToArray();
            if (snoozedReminders.Any())
            {
                foreach (var reminder in snoozedReminders)
                {
                    reminder.State = ReminderState.Pending;
                    reminder.FiredAt = DateTime.UtcNow;
                }

                await snoozedReminders.SaveAsync(cancellation: cancellationToken);

                var newPendingReminders = snoozedReminders.Join(overSnoozedRemindersInfo, r => r.TodoId,
                    x => x.todo!.ID,
                    (r, x) => new PendingReminderDto { OwnerId = x.todo!.OwnerId, Id = r.ID, TodoId = r.TodoId, Title = x.todo!.Title }).ToList();
                string jsonPayload = JsonSerializer.Serialize(newPendingReminders);
                ServiceBusMessage notificationMessage = new ServiceBusMessage(jsonPayload);
                notificationMessage.ApplicationProperties.Add("IsNotification", true);
                notificationMessage.ApplicationProperties.Add("NotificationType", "Add");
                await sender.SendMessageAsync(notificationMessage, cancellationToken);
            }

            var remindersNeedToDimissModel = await DB.Collection<Reminder>()
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
                //.Select(x => x.reminder)
                .ToListAsync(cancellationToken: cancellationToken);

            var remindersNeedToDimiss = remindersNeedToDimissModel.Select(x => x.reminder);

            if (remindersNeedToDimiss.Any())
            {
                foreach (var reminder in remindersNeedToDimiss)
                {
                    reminder.State = ReminderState.Dismissed;
                    reminder.DimissAt = DateTime.UtcNow;
                }

                await remindersNeedToDimiss.SaveAsync(cancellation: cancellationToken);
                string jsonPayload =
                    JsonSerializer.Serialize(remindersNeedToDimissModel.Select(x => new PendingReminderDto { Id = x.reminder.ID, OwnerId = x.todo!.OwnerId }));
                ServiceBusMessage notificationMessage = new ServiceBusMessage(jsonPayload);
                notificationMessage.ApplicationProperties.Add("IsNotification", true);
                notificationMessage.ApplicationProperties.Add("NotificationType", "Remove");
                await sender.SendMessageAsync(notificationMessage, cancellationToken);
            }
        }
    }
}
