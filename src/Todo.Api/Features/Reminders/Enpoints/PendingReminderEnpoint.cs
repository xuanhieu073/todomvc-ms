using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos;
using MongoDB.Driver.Linq;
using AutoMapper;
using Todo.Api.Common;

namespace Todo.Api.Features.Reminders.Enpoints;

public partial class ReminderEndpoints
{
    public void AddPendingReminderRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("", async ([AsParameters] PendingReminderQuery query, ISender sender)
            => await sender.Send(query));
    }

    public sealed record PendingReminderQuery(string? State) : UserBoundRequest, IRequest<List<PendingReminderDto>>;

    public class PendingReminderHandler(IMapper mapper)
        : IRequestHandler<PendingReminderQuery, List<PendingReminderDto>>
    {
        public async Task<List<PendingReminderDto>> Handle(PendingReminderQuery request,
            CancellationToken cancellationToken)
        {
            var query = DB.Queryable<Reminder>()
                .Join(DB.Collection<TodoItem>(), r => r.TodoId, td => td.ID,
                    (reminder, todo) => new ReminderTodoDto { Reminder = reminder, Todo = todo })
                .Where(rdto => rdto.Todo.OwnerId == request.UserId);
            if (request.State == null)
            {
                var allreminders = await query.ToListAsync(cancellationToken: cancellationToken);
                return mapper.Map<List<PendingReminderDto>>(allreminders);
            }

            query = request.State.ToLower() switch
            {
                "pending" => query.Where(r => r.Reminder.State == ReminderState.Pending),
                "snoozed" => query.Where(r => r.Reminder.State == ReminderState.Snoozed),
                "dimissed" => query.Where(r => r.Reminder.State == ReminderState.Dismissed),
                _ => query
            };
            var reminders = await query.ToListAsync(cancellationToken: cancellationToken);
            return mapper.Map<List<PendingReminderDto>>(reminders);
        }
    }
}