using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Statistics.Endpoints;

public class GetStatisticEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/bff/stats/overview",
            async ([AsParameters] GetStatisticsQuery query, ISender sender) =>
            (await sender.Send(query)).ToHttpResponse());
    }

    public sealed record GetStatisticsQuery : IRequest<ApiResult>;

    public class GetStatisticsHandler(StatisticApiClient apiClient) : IRequestHandler<GetStatisticsQuery, ApiResult>
    {
        public async Task<ApiResult> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            var response = await apiClient.GetStatistics(cancellationToken);
            return response;
        }
    }
}