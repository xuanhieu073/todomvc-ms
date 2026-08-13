using MediatR;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Todo.Api.Common;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders.Enpoints;

public partial class ReminderEndpoints
{
    public void AddDimissReminderRoute(IEndpointRouteBuilder app)
    {
        app.MapPatch("/{id}/dimiss", async ([AsParameters] DimissReminderCommand command, ISender sender)
            => await sender.Send(command));
    }

    public sealed record DimissReminderCommand(string Id) : UserBoundRequest, IRequest<Reminder?>;

    public class DimissReminderHandler : IRequestHandler<DimissReminderCommand, Reminder?>
    {
        public async Task<Reminder?> Handle(DimissReminderCommand request, CancellationToken cancellationToken)
        {
            var reminder = await DB.Queryable<Reminder>()
                .Join(DB.Collection<TodoItem>(), r => r.TodoId, t => t.ID, (r, t) => new { r, t })
                .Where(x => x.t.OwnerId == request.UserId && x.r.ID == request.Id)
                .Select(x => x.r)
                .FirstOrDefaultAsync(cancellationToken);

            if (reminder == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }

            reminder.State = ReminderState.Dismissed;
            reminder.DimissAt = DateTime.UtcNow;
            await reminder.SaveAsync(cancellation: cancellationToken);
            return reminder;
        }
    }
}