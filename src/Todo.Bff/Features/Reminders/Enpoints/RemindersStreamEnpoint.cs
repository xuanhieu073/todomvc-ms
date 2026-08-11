using System.Runtime.CompilerServices;
using Carter;
using System.Net.ServerSentEvents;
using System.Threading.Channels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Todo.Bff.Features.Reminders.Enpoints;

public class RemindersStream : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/bff/reminders/stream", (
            NotificationBroker notificationBroker,
            CancellationToken cancellationToken) =>
        {
            try
            {
                return Task.FromResult(
                    TypedResults.ServerSentEvents(
                        MapToSseMessage(notificationBroker.ToAsyncEnumerable(cancellationToken: cancellationToken))));
            }
            catch (Exception exception)
            {
                return Task.FromException<ServerSentEventsResult<List<PendingReminderDto>>>(exception);
            }
        });

        static async IAsyncEnumerable<SseItem<List<PendingReminderDto>>> MapToSseMessage(
            IAsyncEnumerable<NotificaitonEvent> stream)
        {
            await foreach (var data in stream)
            {
                if (data is FiredNotificationEvent)
                {
                    yield return new SseItem<List<PendingReminderDto>>(
                        data: data.Reminders,
                        eventType: "reminder-fired"
                    );
                }
                else if (data is RemovedNotificationEvent)
                {
                    yield return new SseItem<List<PendingReminderDto>>(
                        data: data.Reminders,
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