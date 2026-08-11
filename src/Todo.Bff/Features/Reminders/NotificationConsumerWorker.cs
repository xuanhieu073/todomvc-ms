using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Todo.Bff.Features.Reminders;

public class NotificationBroker : RxEventBroker<NotificaitonEvent>;

public class NotificaitonEvent
{
    public List<PendingReminderDto> Reminders { get; set; }
}

public class FiredNotificationEvent : NotificaitonEvent;

public class RemovedNotificationEvent : NotificaitonEvent;

public class NotificationConsumerWorker(
    ServiceBusClient client,
    NotificationBroker notificationBroker) : BackgroundService
{
    private readonly ServiceBusProcessor _processor =
        client.CreateProcessor("queue.notification", new ServiceBusProcessorOptions());

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string jsonBody = args.Message.Body.ToString();

        try
        {
            var list = JsonSerializer.Deserialize<List<PendingReminderDto>>(jsonBody);

            if (list != null)
            {
                if (args.Message.ApplicationProperties.TryGetValue("NotificationType", out var value))
                {
                    string notificationType = (string)value;
                    if (notificationType == "Add")
                    {
                        notificationBroker.Publish(new FiredNotificationEvent { Reminders = list });
                    }
                    else
                    {
                        notificationBroker.Publish(new RemovedNotificationEvent { Reminders = list });
                    }
                }
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize message: {ex.Message}");
            await args.DeadLetterMessageAsync(args.Message, "InvalidJSON", ex.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args) => Task.CompletedTask;
}