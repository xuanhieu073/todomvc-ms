using AutoMapper;
using MediatR;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Todo.Api.Common;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders.Enpoints;

public partial class ReminderEndpoints
{
    public void AddDimissedReminderRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/dimissed", async ([AsParameters] DimissedRemindersQuery query, ISender sender)
            => await sender.Send(query));
    }

    public sealed record DimissedRemindersQuery(DateTime From) : UserBoundRequest, IRequest<List<ReminderDto>>;

    public class DimissedRemindersHandler(IMapper mapper) : IRequestHandler<DimissedRemindersQuery, List<ReminderDto>>
    {
        public async Task<List<ReminderDto>> Handle(DimissedRemindersQuery request, CancellationToken cancellationToken)
        {
            var dimissedReminders = await DB.Queryable<Reminder>()
                .Join(DB.Collection<TodoItem>(), r => r.TodoId, t => t.ID, (r, t) => new { r, t })
                .Where(x => x.t.OwnerId == request.UserId && x.r.State == ReminderState.Dismissed &&
                            x.r.DimissAt >= request.From)
                .Select(x => x.r).ToListAsync(cancellationToken: cancellationToken);
            return mapper.Map<List<ReminderDto>>(dimissedReminders);
        }
    }
}