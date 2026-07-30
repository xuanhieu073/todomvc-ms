using AutoMapper;
using MediatR;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using MongoDB.Driver;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Reminders.Application.Queries;

public class PendingReminderHandler(IMapper _mapper) : IRequestHandler<PendingReminderQuery, List<PendingReminderDto>>
{
    public async Task<List<PendingReminderDto>> Handle(PendingReminderQuery request, CancellationToken cancellationToken)
    {
        var query = DB.Queryable<Reminder>().Join(DB.Collection<TodoItem>(), r => r.TodoId, td => td.ID, (reminder, todo) => new ReminderTodoDto { Reminder = reminder, Todo = todo });
        query = request.state.ToLower() switch
        {
            "pending" => query.Where(r => r.Reminder.State == ReminderState.Pending && r.Reminder.FiredAt == null),
            _ => query
        };
        var reminders = await query.ToListAsync();
        return _mapper.Map<List<PendingReminderDto>>(reminders);
    }
}