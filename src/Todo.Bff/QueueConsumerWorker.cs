using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Todo.Bff.Features.Reminders;

public class QueueConsumerWorker : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ServiceBusProcessor _processor2;
    private readonly Queue1Broker _rxBroker;
    private readonly Queue2Broker _rxBroker2;

    public QueueConsumerWorker(ServiceBusClient client,
            Queue1Broker rxBroker,
            Queue2Broker rxBroker2)
    {
        _rxBroker = rxBroker;
        _rxBroker2 = rxBroker2;
        _processor = client.CreateProcessor("queue.1", new ServiceBusProcessorOptions());
        _processor2 = client.CreateProcessor("queue.2", new ServiceBusProcessorOptions());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync(stoppingToken);

        _processor2.ProcessMessageAsync += MessageHandler2;
        _processor2.ProcessErrorAsync += ErrorHandler;
        await _processor2.StartProcessingAsync(stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string jsonBody = args.Message.Body.ToString();

        try
        {
            // 2. Deserialize back to List<string> [1]
            var list = JsonSerializer.Deserialize<List<PendingReminderDto>>(jsonBody);

            if (list != null)
            {
                // 3. Publish the list to the Rx Broker
                _rxBroker.Publish(list);
            }

            // 4. Complete the message [3]
            await args.CompleteMessageAsync(args.Message);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize message: {ex.Message}");
            // Optionally dead-letter the malformed message so it's not retried indefinitely [4]
            await args.DeadLetterMessageAsync(args.Message, "InvalidJSON", ex.Message);
        }
    }

    private async Task MessageHandler2(ProcessMessageEventArgs args)
    {
        string jsonBody = args.Message.Body.ToString();

        try
        {
            // 2. Deserialize back to List<string> [1]
            var list = JsonSerializer.Deserialize<List<PendingReminderDto>>(jsonBody);

            if (list != null)
            {
                // 3. Publish the list to the Rx Broker
                _rxBroker2.Publish(list);
            }

            // 4. Complete the message [3]
            await args.CompleteMessageAsync(args.Message);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize message: {ex.Message}");
            // Optionally dead-letter the malformed message so it's not retried indefinitely [4]
            await args.DeadLetterMessageAsync(args.Message, "InvalidJSON", ex.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args) => Task.CompletedTask;
}