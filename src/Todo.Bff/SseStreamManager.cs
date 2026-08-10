using System.Collections.Concurrent;
using System.Net.ServerSentEvents;
using System.Threading.Channels;
using Todo.Bff.Features.Reminders;

public class SseMessage
{
    public string Data { get; set; }
    public string? Event { get; set; }
    public string? Id { get; set; }
    public SseMessage(string data)
    {
        Data = data;
    }
}

public class SseStreamManager
{
    private readonly ConcurrentDictionary<Guid, Channel<SseMessage>> _clients = new();

    // Pushes messages directly into all active client streams
    public async Task BroadcastMessageAsync(string data)
    {
        var sseMessage = new SseMessage(data)
        {
            Event = "message",
            Id = Guid.NewGuid().ToString()
        };

        foreach (var clientChannel in _clients.Values)
        {
            await clientChannel.Writer.WriteAsync(sseMessage);
        }
    }

    // Exposes the SseMessages as an IAsyncEnumerable stream for TypedResults
    public async IAsyncEnumerable<SseItem<List<PendingReminderDto>>> GetServerEventsAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var clientId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<SseMessage>();
        _clients.TryAdd(clientId, channel);

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                //yield return message;
                yield return new SseItem<List<PendingReminderDto>>(
                    data: new List<PendingReminderDto>(),
                    eventType: "receive"
                );
                yield return new SseItem<List<PendingReminderDto>>(
                    data: new List<PendingReminderDto>(),
                    eventType: "remove"
                );
            }
        }
        finally
        {
            _clients.TryRemove(clientId, out _);
        }
    }
}