using System.Net;

namespace Todo.Bff.Common;

public sealed class RetryHandler : DelegatingHandler
{
    private const int MaxRetries = 3;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt <= MaxRetries; attempt++)
        {
            try
            {
                var response = await base.SendAsync(
                    request,
                    cancellationToken);

                if (IsTransient(response) && attempt < MaxRetries)
                {
                    response.Dispose();

                    await Task.Delay(
                        TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                        cancellationToken);

                    continue;
                }

                return response;
            }
            catch (HttpRequestException) when (attempt < MaxRetries)
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    cancellationToken);
            }
        }

        throw new InvalidOperationException();
    }

    private static bool IsTransient(HttpResponseMessage response)
    {
        return response.StatusCode is
            HttpStatusCode.RequestTimeout or
            HttpStatusCode.TooManyRequests or
            HttpStatusCode.InternalServerError or
            HttpStatusCode.BadGateway or
            HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout;
    }
}