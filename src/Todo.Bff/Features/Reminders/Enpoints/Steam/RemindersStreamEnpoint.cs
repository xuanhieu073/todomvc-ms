using System.Runtime.CompilerServices;
using Carter;
using MediatR;
using Todo.Bff.Features.Reminders.Application.Commands;
using Todo.Bff.Features.Reminders.Application.Queries;
using Todo.Bff.Features.Reminders.DTOs;

namespace Todo.Bff.Features.Reminders.Endpoint.Stream;

public class RemindersStream : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/bff/reminders/stream", (ISender sender) =>
        {
            // Returns a built-in ServerSentEvents result
            return TypedResults.ServerSentEvents(GetServerEventsAsync(sender), eventType: "remindersUpdate");
        });

        // A helper method generating fake real-time event objects 
        async static IAsyncEnumerable<List<PendingReminderDto>> GetServerEventsAsync(
            ISender sender,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            TimeSpan _delayInterval = TimeSpan.FromSeconds(2);
            var random = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_delayInterval, cancellationToken);

                var query = new RemindersStreamQuery(ReminderState.Pending);
                var response = await sender.Send(query);

                foreach (var reminder in response)
                {
                    var command = new UpdateFireAtCommand(reminder.Id);
                    await sender.Send(command);
                }

                yield return new List<PendingReminderDto>(response);
            }
        }
    }
}