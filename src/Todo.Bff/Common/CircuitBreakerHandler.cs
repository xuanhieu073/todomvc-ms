namespace Todo.Bff.Common;

public sealed class CircuitBreakerState
{
    private readonly object _lock = new();

    private int _failureCount;
    private DateTime _openUntil = DateTime.MinValue;

    public bool IsOpen
    {
        get
        {
            lock (_lock)
            {
                return DateTime.UtcNow < _openUntil;
            }
        }
    }

    public void RegisterFailure(int threshold, TimeSpan breakDuration)
    {
        lock (_lock)
        {
            _failureCount++;

            if (_failureCount >= threshold)
            {
                _openUntil = DateTime.UtcNow + breakDuration;
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _failureCount = 0;
            _openUntil = DateTime.MinValue;
        }
    }
}

public sealed class CircuitBreakerHandler(
    CircuitBreakerState state) : DelegatingHandler
{
    private const int FailureThreshold = 5;

    private static readonly TimeSpan BreakDuration =
        TimeSpan.FromSeconds(30);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (state.IsOpen)
        {
            throw new HttpRequestException(
                "Circuit breaker is open.");
        }

        try
        {
            var response = await base.SendAsync(
                request,
                cancellationToken);

            if ((int)response.StatusCode >= 500)
            {
                state.RegisterFailure(
                    FailureThreshold,
                    BreakDuration);
            }
            else
            {
                state.Reset();
            }

            return response;
        }
        catch (HttpRequestException)
        {
            state.RegisterFailure(
                FailureThreshold,
                BreakDuration);

            throw;
        }
    }
}