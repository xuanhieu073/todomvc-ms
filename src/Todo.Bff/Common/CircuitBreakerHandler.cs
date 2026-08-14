namespace Todo.Bff.Common;

public sealed class CircuitBreakerHandler : DelegatingHandler
{
    private readonly Lock _lock = new();

    private int _failureCount;
    private DateTime _openUntil = DateTime.MinValue;

    private const int FailureThreshold = 5;
    private static readonly TimeSpan BreakDuration = TimeSpan.FromSeconds(30);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (DateTime.UtcNow < _openUntil)
            {
                throw new HttpRequestException(
                    "Circuit breaker is open.");
            }
        }

        try
        {
            var response = await base.SendAsync(
                request,
                cancellationToken);

            if (IsFailure(response))
            {
                RegisterFailure();
            }
            else
            {
                Reset();
            }

            return response;
        }
        catch (HttpRequestException)
        {
            RegisterFailure();
            throw;
        }
    }

    private void RegisterFailure()
    {
        lock (_lock)
        {
            _failureCount++;

            if (_failureCount >= FailureThreshold)
            {
                _openUntil = DateTime.UtcNow + BreakDuration;
            }
        }
    }

    private void Reset()
    {
        lock (_lock)
        {
            _failureCount = 0;
        }
    }

    private static bool IsFailure(HttpResponseMessage response)
    {
        return (int)response.StatusCode >= 500;
    }
}