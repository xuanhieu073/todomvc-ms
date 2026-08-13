namespace Todo.Bff.Clients;

public record LoginResponse(string Email, string AccessToken);

public class AuthApiClient(HttpClient httpClient) : ApiClient(httpClient)
{
    public Task<ApiResult> Login<TRequest>(TRequest loginRequest, CancellationToken cancellationToken = default)
        => SendAsync<LoginResponse>(HttpMethod.Post, "/api/auth/login", loginRequest, cancellationToken);

    public Task<ApiResult> Register<TRequest>(TRequest loginRequest, CancellationToken cancellationToken = default)
        => SendAsync<LoginResponse>(HttpMethod.Post, "/api/auth/register", loginRequest, cancellationToken);
}