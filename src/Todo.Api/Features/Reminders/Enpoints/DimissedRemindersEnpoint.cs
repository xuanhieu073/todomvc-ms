using AutoMapper;
using Carter;
using MediatR;
using MongoDB.Entities;

namespace Todo.Api.Features.Reminders.Enpoints;

public class DimissedReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reminders/dimissed", ([AsParameters] DimissedRemindersQuery query, ISender sender) =>
        sender.Send(query));
    }

    public sealed record DimissedRemindersQuery(DateTime From) : IRequest<List<ReminderDto>>;

    public class DimissedRemindersHandler(IMapper mapper) : IRequestHandler<DimissedRemindersQuery, List<ReminderDto>>
    {
        public async Task<List<ReminderDto>> Handle(DimissedRemindersQuery request, CancellationToken cancellationToken)
        {
            var dimissedReminders = await DB.Find<Reminder>()
                .Match(r => r.State == ReminderState.Dismissed && r.DimissAt >= request.From)
                .ExecuteAsync(cancellationToken);
            return mapper.Map<List<ReminderDto>>(dimissedReminders);
        }
    }
}