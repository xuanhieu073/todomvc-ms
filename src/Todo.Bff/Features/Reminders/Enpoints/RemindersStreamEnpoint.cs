using System.Runtime.CompilerServices;
using Carter;
using MediatR;
using Todo.Bff.Clients;
using System.Net.ServerSentEvents;

namespace Todo.Bff.Features.Reminders.Enpoints;

public class RemindersStream : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/bff/reminders/stream", (ISender sender, ILogger<PendingReminderDto> logger) =>
        {
            return TypedResults.ServerSentEvents(GetServerEventsAsync(sender));
        });

        // A helper method generating fake real-time event objects 
        async static IAsyncEnumerable<SseItem<List<PendingReminderDto>>> GetServerEventsAsync(
            ISender sender,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            TimeSpan _delayInterval = TimeSpan.FromSeconds(2);

            DateTime? fireAt = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (fireAt == null) { fireAt = DateTime.UtcNow; }
                var fireAtString = ((DateTime)fireAt).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
                await Task.Delay(_delayInterval, cancellationToken);

                var query = new GetUpcomingRemindersQuery("60s", fireAtString);
                var response = await sender.Send(query);

                var dimissedquery = new GetDimissedRemindersQuery(fireAtString);
                var dimissedresponse = await sender.Send(dimissedquery);


                fireAt = DateTime.UtcNow;
                yield return new SseItem<List<PendingReminderDto>>(
                    data: response,
                    eventType: "receive"
                );
                yield return new SseItem<List<PendingReminderDto>>(
                    data: dimissedresponse.Select(d => new PendingReminderDto { Id = d.Id}).ToList(),
                    eventType: "remove"
                );
            }
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

    public class GetDimissedRemindersyHandler(ReminderApiClient _apiClient) : IRequestHandler<GetDimissedRemindersQuery,List<ReminderDto>>
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