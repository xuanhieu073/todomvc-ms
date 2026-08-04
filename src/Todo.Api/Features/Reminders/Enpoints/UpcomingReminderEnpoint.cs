using AutoMapper;
using Carter;
using MediatR;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System.Text.RegularExpressions;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders.Enpoints;

public class UpcomingReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reminders/upcoming", async ([AsParameters] UpcomingReminderQuery query, ISender sender) =>
            await sender.Send(query));
    }

    public sealed record UpcomingReminderQuery(string? within, DateTime? fireAt) : IRequest<List<PendingReminderDto>>;

    public class UpcomingReminderHandler(IMapper _mapper) : IRequestHandler<UpcomingReminderQuery, List<PendingReminderDto>>
    {
        public async Task<List<PendingReminderDto>> Handle(UpcomingReminderQuery request, CancellationToken cancellationToken)
        {

            var query = DB.Queryable<Reminder>().Join(DB.Collection<TodoItem>(), r => r.TodoId, td => td.ID, (reminder, todo) => new ReminderTodoDto { Reminder = reminder, Todo = todo });
            if (request.within == null)
            {
                var reminders = await query.ToListAsync();
                return _mapper.Map<List<PendingReminderDto>>(reminders);
            }

            string pattern = @"^(?<number>\d+)(?<unit>[a-zA-Z]+)$";

            Match match = Regex.Match(request.within, pattern);

            if (match.Success)
            {
                string number = match.Groups["number"].Value;
                string unit = match.Groups["unit"].Value;

                int.TryParse(number, out int value);

                var now = DateTime.UtcNow;
                query = unit switch
                {
                    "h" => query.Where(x => x.Reminder.State == ReminderState.Pending && x.Todo.DueAt <= now.AddHours(value)),
                    "m" => query.Where(x => x.Reminder.State == ReminderState.Pending && x.Todo.DueAt <= now.AddMinutes(value)),
                    "s" => query.Where(x => (request.fireAt == null || x.Reminder.FiredAt > request.fireAt) && x.Reminder.State == ReminderState.Pending && x.Todo.DueAt <= now.AddSeconds(value)),
                    _ => query,
                };
                var reminders = await query.ToListAsync();
                return _mapper.Map<List<PendingReminderDto>>(reminders);
            }
            return new List<PendingReminderDto>();
        }
    }
}