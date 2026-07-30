using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System.Text.RegularExpressions;
using Todo.Api.Features.Todos.Entities;

namespace Todo.Api.Features.Reminders.Application.Queries;

public class UpcomingReminderHandler : IRequestHandler<UpcomingReminderQuery, List<TodoItem>>
{
    public async Task<List<TodoItem>> Handle(UpcomingReminderQuery request, CancellationToken cancellationToken)
    {
        string pattern = @"^(?<number>\d+)(?<unit>[a-zA-Z]+)$";

        Match match = Regex.Match(request.within, pattern);

        if (match.Success)
        {
            string number = match.Groups["number"].Value;
            string unit = match.Groups["unit"].Value;
                
            int.TryParse(number, out int value);

            var now = DateTime.Now;
            var query = DB.Queryable<TodoItem>()
            .GroupJoin(
                DB.Collection<Reminder>(),
                td => td.ID,
                r => r.TodoId,
                (todo, reminders) => new { todo, reminders }
            )
            .SelectMany(
                x => x.reminders.DefaultIfEmpty(),
                (x, reminder) => new { x.todo, reminder }
            );
            query = unit switch
            {
                "h" => query.Where(x => x.todo.DueAt >= now && x.todo.DueAt <= now.AddHours(value)),
                "m" => query.Where(x => x.todo.DueAt >= now && x.todo.DueAt <= now.AddMinutes(value)),
                "s" => query.Where(x => x.todo.DueAt >= now && x.todo.DueAt <= now.AddSeconds(value)),
                _ => query,
            };
            return await query.Select(x => x.todo).ToListAsync();
        }
        return new List<TodoItem>();
    }
}