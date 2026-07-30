using AutoMapper;
using MediatR;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace Todo.Api.Features.Reminders.Application.Queries;

public class PendingReminderHandler(IMapper mapper) : IRequestHandler<PendingReminderQuery, List<ReminderDto>>
{
    public async Task<List<ReminderDto>> Handle(PendingReminderQuery request, CancellationToken cancellationToken)
    {
        var query = DB.Queryable<Reminder>();
        query = request.state.ToLower() switch
        {
            "pending" => query.Where(r => r.State == ReminderState.Pending && r.FiredAt == null),
            _ => query
        };
        var reminders = await query.ToListAsync();
        return reminders.Select(mapper.Map<ReminderDto>).ToList();
    }
}