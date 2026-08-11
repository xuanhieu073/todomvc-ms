namespace Todo.Bff.Clients;

public class StatisticApiClient(HttpClient httpClient) : ApiClient(httpClient)
{
    public Task<ApiResult> GetStatistics(CancellationToken ct = default)
        => GetAsync<StatsOverviewResponse>("/api/stats/overview", ct);
}