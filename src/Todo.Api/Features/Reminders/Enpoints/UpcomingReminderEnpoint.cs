using AutoMapper;
using Carter;
using MediatR;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System.Text.RegularExpressions;
using Todo.Api.Common;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders.Enpoints;

public partial class ReminderEndpoints
{
    public void AddUpcompingRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/upcoming", async ([AsParameters] UpcomingReminderQuery query, ISender sender) =>
        await sender.Send(query));
    }

    public sealed record UpcomingReminderQuery(string? Within, DateTime? FireAt)
        : UserBoundRequest, IRequest<List<PendingReminderDto>>;

    public class UpcomingReminderHandler(IMapper mapper)
        : IRequestHandler<UpcomingReminderQuery, List<PendingReminderDto>>
    {
        public async Task<List<PendingReminderDto>> Handle(UpcomingReminderQuery request,
            CancellationToken cancellationToken)
        {
            var query = DB.Queryable<Reminder>().Join(DB.Collection<TodoItem>(), r => r.TodoId, td => td.ID,
                    (reminder, todo) => new ReminderTodoDto { Reminder = reminder, Todo = todo })
                .Where(rdto => rdto.Todo.OwnerId == request.UserId);
            if (request.Within == null)
            {
                var reminders = await query.ToListAsync(cancellationToken: cancellationToken);
                return mapper.Map<List<PendingReminderDto>>(reminders);
            }

            string pattern = @"^(?<number>\d+)(?<unit>[a-zA-Z]+)$";

            Match match = Regex.Match(request.Within, pattern);

            if (match.Success)
            {
                string number = match.Groups["number"].Value;
                string unit = match.Groups["unit"].Value;

                int.TryParse(number, out int value);

                var now = DateTime.UtcNow;
                query = unit switch
                {
                    "h" => query.Where(x =>
                        x.Reminder.State == ReminderState.Pending && x.Todo.DueAt <= now.AddHours(value)),
                    "m" => query.Where(x =>
                        x.Reminder.State == ReminderState.Pending && x.Todo.DueAt <= now.AddMinutes(value)),
                    "s" => query.Where(x =>
                        (request.FireAt == null || x.Reminder.FiredAt > request.FireAt) &&
                        x.Reminder.State == ReminderState.Pending && x.Todo.DueAt <= now.AddSeconds(value)),
                    _ => query,
                };
                var reminders = await query.ToListAsync(cancellationToken: cancellationToken);
                return mapper.Map<List<PendingReminderDto>>(reminders);
            }

            return new List<PendingReminderDto>();
        }
    }
}