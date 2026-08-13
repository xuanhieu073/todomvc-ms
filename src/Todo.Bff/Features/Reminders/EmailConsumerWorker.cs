using System.Text.Json;
using Azure.Messaging.ServiceBus;
using FluentEmail.Core;

namespace Todo.Bff.Features.Reminders
{
    public class EmailConsumerWorker(ServiceBusClient client, IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly ServiceBusProcessor _emailProcessor =
            client.CreateProcessor("queue.email", new ServiceBusProcessorOptions());

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _emailProcessor.ProcessMessageAsync += MessageHandler;
            _emailProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailProcessor.StartProcessingAsync(stoppingToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string jsonBody = args.Message.Body.ToString();

            try
            {
                var list = JsonSerializer.Deserialize<List<PendingReminderDto>>(jsonBody);
                var todoGropuByUser = list?.GroupBy(x => x.Id);
                if (todoGropuByUser != null)
                {
                    foreach (var todosByUser in todoGropuByUser)
                    {
                        using var scope = serviceProvider.CreateScope();
                        var fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                        var response = await fluentEmail
                            .To(todosByUser.First().OwnerEmail)
                            .Subject("Quick Reminder")
                            .Body(
                                $"<h1>Just a quick reminder that you need to finish {string.Join(",", todosByUser.Select(x => x.Title))}</h1>",
                                isHtml: true)
                            .SendAsync();

                        if (!response.Successful)
                        {
                            throw new Exception(string.Join(",", response.ErrorMessages));
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
}
