using Carter;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Reminders.Enpoints;

public class DimissReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/reminders/{id}/dimiss", async ([AsParameters] DimissReminderCommand command, ISender sender) =>
            await sender.Send(command));
    }
    public sealed record DimissReminderCommand(string Id) : IRequest<Reminder?>;
    public class DimissReminderHandler : IRequestHandler<DimissReminderCommand, Reminder?>
    {
        public async Task<Reminder?> Handle(DimissReminderCommand request, CancellationToken cancellationToken)
        {
            var reminder = await DB.Find<Reminder>().OneAsync(request.Id);
            
            if (reminder == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }
            else 
            {
                reminder.State = ReminderState.Dismissed;
                reminder.DimissAt = DateTime.Now;
                await reminder.SaveAsync();
                return reminder;
            }
        }
    }
}