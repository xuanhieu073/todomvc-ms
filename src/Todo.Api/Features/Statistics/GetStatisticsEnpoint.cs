using Carter;
using MediatR;
using MongoDB.Driver;
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
        var pipeline = new Template<TodoItem, StatsOverviewDto>(@"
        [
            { 
                '$group': { 
                    '_id': null, 
                    '<Total>': { '$sum': 1 },
                    '<Active>': {
                        '$sum': {
                        '$cond': [
                            {
                            '$eq': [
                                '$IsCompleted',
                                false
                            ]
                            },
                            1,
                            0
                            ]
                        }
                    },
                    '<Completed>': {
                        '$sum': {
                        '$cond': [
                            {
                            '$eq': [
                                '$IsCompleted',
                                true
                            ]
                            },
                            1,
                            0
                        ]
                        }
                    },
                    '<Overdue>': {
                        '$sum': {
                        '$cond': [
                            {
                            '$and': [
                                {
                                '$eq': [
                                    '$IsCompleted',
                                    false
                                ]
                                },
                                {
                                '$lte': [
                                    '$DueAt',
                                    '$$NOW'
                                ]
                                }
                            ]
                            },
                            1,
                            0
                        ]
                        }
                    },
                    '<CompletedToday>': {
                        '$sum': {
                        '$cond': [
                            {
                            '$eq': [
                                {
                                '$dateTrunc': {
                                    'date': '$CompletedAt',
                                    'unit': 'day',
                                    'timezone': 'Asia/Saigon'
                                }
                                },
                                {
                                '$dateTrunc': {
                                    'date': '$$NOW',
                                    'unit': 'day',
                                    'timezone': 'Asia/Saigon'
                                }
                                }
                            ]
                            },
                            1,
                            0
                        ]
                        }
                    },
                    '<CompletedThisWeek>': {
                        '$sum': {
                        '$cond': [
                            {
                            '$eq': [
                                {
                                '$dateTrunc': {
                                    'date': '$CompletedAt',
                                    'unit': 'week',
                                    'timezone': 'Asia/Saigon'
                                }
                                },
                                {
                                '$dateTrunc': {
                                    'date': '$$NOW',
                                    'unit': 'week',
                                    'timezone': 'Asia/Saigon'
                                }
                                }
                            ]
                            },
                            1,
                            0
                        ]
                        }
                    }
                } 
            },
            {
                '$project': {
                    '_id': 0,
                    '<Total>': 1,
                    '<Active>': 1,
                    '<Completed>': 1,
                    '<Overdue>': 1,
                    '<CompletedToday>': 1,
                    '<CompletedThisWeek>': 1,
                    '<CompletionRate>': {
                        '$cond': [
                        {
                            '$eq': [
                            '$<Total>',
                            0
                            ]
                        },
                        0,
                        {
                            '$round': [
                            {
                                '$divide': [
                                '$<Completed>',
                                '$<Total>'
                                ]
                            },
                            4
                            ]
                        }
                        ]
                    }
                }
            }
        ]")
        .Tag("Total", nameof(StatsOverviewDto.Total))
        .Tag("Active", nameof(StatsOverviewDto.Active))
        .Tag("Completed", nameof(StatsOverviewDto.Completed))
        .Tag("Overdue", nameof(StatsOverviewDto.Overdue))
        .Tag("CompletedToday", nameof(StatsOverviewDto.CompletedToday))
        .Tag("CompletedThisWeek", nameof(StatsOverviewDto.CompletedThisWeek))
        .Tag("CompletionRate", nameof(StatsOverviewDto.CompletionRate));

        //pipeline.Tag("Total", nameof(StatsOverviewDto.Total));

        //var results = await DB.PipelineAsync<TodoItem, StatsOverviewDto>(pipeline);
        var results = await DB.PipelineCursorAsync(pipeline);
        var statsOverview = results.FirstOrDefault();

        return statsOverview;
    }
}