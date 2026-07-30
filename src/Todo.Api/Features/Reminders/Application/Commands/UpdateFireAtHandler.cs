using AutoMapper;
using MediatR;
using MongoDB.Entities;

namespace Todo.Api.Features.Reminders.Application.Commands;

public class UpdateFireAtHandler(IMapper mapper) : IRequestHandler<UpdateFireAtCommand, ReminderDto?>
{
    public async Task<ReminderDto?> Handle(UpdateFireAtCommand request, CancellationToken cancellationToken)
    {
        var reminder = await DB.Find<Reminder>().OneAsync(request.Id);
        if (reminder == null)
        {
            return null;
        }
        reminder.FiredAt = DateTime.Now;
        await reminder.SaveAsync();
        return mapper.Map<ReminderDto>(reminder);
    }
}