using MediatR;
using MongoDB.Entities;

namespace Todo.Api.Features.Reminders.Application.Commands;

public class SnoozeReminderHandler : IRequestHandler<SnoozeReminderCommand, Reminder?>
{
    public async Task<Reminder?> Handle(SnoozeReminderCommand request, CancellationToken cancellationToken)
    {
        var reminder = await DB.Find<Reminder>().OneAsync(request.Id);
        if (reminder != null)
        {
            reminder.State = ReminderState.Snoozed;
            await reminder.SaveAsync();
            return reminder;
        }
        return null;
    }
}