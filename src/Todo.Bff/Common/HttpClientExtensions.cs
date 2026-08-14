using Todo.Bff.Clients;

namespace Todo.Bff.Common;

public static class HttpClientExtensions
{
    public static IHttpClientBuilder AddTodoApiClient<TClient>(
        this IServiceCollection services,
        bool useAuth = true)
        where TClient : class
    {
        var builder = services
            .AddHttpClient<TClient>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                client.BaseAddress = new Uri(
                    configuration["TodoApi:BaseUrl"]!);

                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<RetryHandler>()
            .AddHttpMessageHandler<CircuitBreakerHandler>();

        if (useAuth)
        {
            builder.AddHttpMessageHandler<ClientAuthDelegatingHandler>();
        }

        return builder;
    }
}