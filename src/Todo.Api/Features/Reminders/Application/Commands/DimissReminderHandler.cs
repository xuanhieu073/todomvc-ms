using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Reminders;

public class DimissReminderHandler : IRequestHandler<DimissReminderCommand, Reminder?>
{
    public async Task<Reminder?> Handle(DimissReminderCommand request, CancellationToken cancellationToken)
    {
        var reminder = await DB.Find<Reminder>().OneAsync(request.Id);
        if (reminder != null)
        {
            reminder.State = ReminderState.Dismissed;
            await reminder.SaveAsync();
            return reminder;
        }
        return null;
    }
}