using Carter;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Statistics;

public class GetStatisticsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/stats/overview", async (ISender sender) =>
            await sender.Send(new GetStatisticsQuery()));
    }
}

public sealed record GetStatisticsQuery() : IRequest<StatsOverviewDto>;

public class GetStatisticsHandler : IRequestHandler<GetStatisticsQuery, StatsOverviewDto>
{
    public async Task<StatsOverviewDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        var pipeline = new Template<TodoItem>(@"
            [
                { $match: { <Title>: '<todo_title>' } } 
            ]")
            .Path(t => t.Title)
            .Tag("todo_title", "Angular");

        // Execute the raw pipeline against your database
        var results = await DB.PipelineAsync<TodoItem, TodoItem>(pipeline);
        return new StatsOverviewDto(Total: 100, Active: 50, Completed: 50, Overdue: 10, CompletedToday: 5, CompletedThisWeek: 20, CompletionRate: 0.5, CompletedByDay: new List<DailyCountDto>());
    }
}