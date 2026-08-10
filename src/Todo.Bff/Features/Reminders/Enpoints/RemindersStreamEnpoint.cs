using System.Runtime.CompilerServices;
using Carter;
using MediatR;
using Todo.Bff.Clients;
using System.Net.ServerSentEvents;
using Azure.Messaging.ServiceBus;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using System.Reactive.Linq;

namespace Todo.Bff.Features.Reminders.Enpoints;

public record TaggedReminderStream(string Source, List<PendingReminderDto> Reminders);
public class RemindersStream : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // app.MapGet("/bff/reminders/stream", async (ISender sender, ILogger<PendingReminderDto> logger, SseStreamManager streamManager) =>
        // {
        //     // return TypedResults.ServerSentEvents(streamManager.GetServerEventsAsync());
        //     // return TypedResults.ServerSentEvents(GetServerEventsAsync(sender, serviceBusClient));
        // });

        app.MapGet("/bff/reminders/stream", async (
            Queue1Broker broker1,
            Queue2Broker broker2,
            CancellationToken cancellationToken) =>
        {
            IObservable<TaggedReminderStream> taggedStream1 = broker1
                .Select(data => new TaggedReminderStream("Queue1", data));

            IObservable<TaggedReminderStream> taggedStream2 = broker2
                .Select(data => new TaggedReminderStream("Queue2", data));

            IObservable<TaggedReminderStream> combinedRxStream = Observable.Merge(taggedStream1, taggedStream2);

            IAsyncEnumerable<TaggedReminderStream> eventStream = combinedRxStream.ToAsyncEnumerable();

            return TypedResults.ServerSentEvents(MapToSseMessage(eventStream));
        });

        static async IAsyncEnumerable<SseItem<List<PendingReminderDto>>> MapToSseMessage(
            IAsyncEnumerable<TaggedReminderStream> stream)
        {
            await foreach (var data in stream)
            {
                // Now you can easily route or filter based on the source identity!
                if (data.Source == "Queue1")
                {
                    Console.WriteLine(data);
                    yield return new SseItem<List<PendingReminderDto>>(
                        data: data.Reminders,
                        eventType: "receive"
                    );
                }
                else if (data.Source == "Queue2")
                {
                    Console.WriteLine(data);
                    yield return new SseItem<List<PendingReminderDto>>(
                        data: data.Reminders,
                        eventType: "remove"
                    );
                }
            }
        }

        // A helper method generating fake real-time event objects 
        async static IAsyncEnumerable<SseItem<List<PendingReminderDto>>> GetServerEventsAsync(
            ISender sender,
            ServiceBusClient serviceBusClient,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            yield return new SseItem<List<PendingReminderDto>>(
                data: new List<PendingReminderDto>(),
                eventType: "receive"
            );
            yield return new SseItem<List<PendingReminderDto>>(
                data: new List<PendingReminderDto>(),
                eventType: "remove"
            );

            // TimeSpan _delayInterval = TimeSpan.FromSeconds(2);

            // DateTime? fireAt = null;
            // while (!cancellationToken.IsCancellationRequested)
            // {
            //     if (fireAt == null) { fireAt = DateTime.UtcNow; }
            //     var fireAtString = ((DateTime)fireAt).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            //     await Task.Delay(_delayInterval, cancellationToken);

            //     var query = new GetUpcomingRemindersQuery("60s", fireAtString);
            //     var response = await sender.Send(query);

            //     var dimissedquery = new GetDimissedRemindersQuery(fireAtString);
            //     var dimissedresponse = await sender.Send(dimissedquery);


            //     fireAt = DateTime.UtcNow;
            //     yield return new SseItem<List<PendingReminderDto>>(
            //         data: response,
            //         eventType: "receive"
            //     );
            //     yield return new SseItem<List<PendingReminderDto>>(
            //         data: dimissedresponse.Select(d => new PendingReminderDto { Id = d.Id }).ToList(),
            //         eventType: "remove"
            //     );
            // }
        }
    }

    public sealed record GetUpcomingRemindersQuery(string within, string fireAt) : IRequest<List<PendingReminderDto>>;

    public class GetUpcomingRemindersHandler(ReminderApiClient _apiClient) : IRequestHandler<GetUpcomingRemindersQuery, List<PendingReminderDto>>
    {
        public async Task<List<PendingReminderDto>> Handle(GetUpcomingRemindersQuery request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.GetUpcomingReminders(request.within, request.fireAt, cancellationToken);
            return response.StatusCode switch
            {
                200 => ((ApiSucessResult<List<PendingReminderDto>>)response).Data ?? new List<PendingReminderDto>(),
                _ => throw new Exception($"Error fetching reminders: {response.StatusCode} - {response.ErrorMessage}")
            };
        }
    }

    public sealed record GetDimissedRemindersQuery(string dimissedAt) : IRequest<List<ReminderDto>>;

    public class GetDimissedRemindersyHandler(ReminderApiClient _apiClient) : IRequestHandler<GetDimissedRemindersQuery, List<ReminderDto>>
    {
        public async Task<List<ReminderDto>> Handle(GetDimissedRemindersQuery request, CancellationToken cancellationToken)
        {
            var response = await _apiClient.GetDimissedReminders(request.dimissedAt, cancellationToken);
            return response.StatusCode switch
            {
                200 => ((ApiSucessResult<List<ReminderDto>>)response).Data ?? new List<ReminderDto>(),
                _ => throw new Exception($"Error fetching reminders: {response.StatusCode} - {response.ErrorMessage}")
            };
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