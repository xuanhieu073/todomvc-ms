using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.IdentityModel.Tokens.Jwt;
using System.Reactive.Linq;


namespace Todo.Bff.Features.Reminders.Enpoints;

public class RemindersStream : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/bff/reminders/stream", (
            [FromQuery] string token,
            NotificationBroker notificationBroker,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var decocedtoken = handler.ReadJwtToken(token);

                // Look for either the XML schema identifier or the short standard 'sub' type
                var nameIdentifierClaim = decocedtoken.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.NameId);

                string? userId = nameIdentifierClaim?.Value;

                return Task.FromResult(
                    TypedResults.ServerSentEvents(
                        MapToSseMessage(userId,
                            notificationBroker.ToEnumerable())));
            }
            catch (Exception exception)
            {
                return Task.FromException<ServerSentEventsResult<List<PendingReminderDto>>>(exception);
            }
        });

        static async IAsyncEnumerable<SseItem<List<PendingReminderDto>>> MapToSseMessage(
            string? userId,
            IEnumerable<NotificaitonEvent> stream)
        {
             foreach (var data in stream)
            {
                if (data is FiredNotificationEvent)
                {
                    yield return new SseItem<List<PendingReminderDto>>(
                        data: data.Reminders.Where(r => r.OwnerId == userId).ToList(),
                        eventType: "reminder-fired"
                    );
                }
                else if (data is RemovedNotificationEvent && data.Reminders.First().OwnerId == userId)
                {
                    yield return new SseItem<List<PendingReminderDto>>(
                        data: data.Reminders.Where(r => r.OwnerId == userId).ToList(),
                        eventType: "reminder-removed"
                    );
                }
            }
        }
    }
}

public static class ObservableExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
        this IObservable<T> observable,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<T>();

        using var subscription = observable.Subscribe(
            onNext: data => channel.Writer.TryWrite(data),
            onError: ex => channel.Writer.TryComplete(ex),
            onCompleted: () => channel.Writer.TryComplete()
        );

        await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return item;
        }
    }
}