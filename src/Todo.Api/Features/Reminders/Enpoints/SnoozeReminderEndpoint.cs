using FluentValidation;
using MediatR;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using Todo.Api.Common;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders.Enpoints;

public partial class ReminderEndpoints
{
    public void AddSnoozeReminderRoute(IEndpointRouteBuilder app)
    {
        app.MapPatch("/{id}/snooze", async (string id, SnoozeReminderCommand command, ISender sender)
            => await sender.Send(command with { Id = id }));
    }

    public sealed record SnoozeReminderCommand(string Id, int Minutes) : UserBoundRequest, IRequest<Reminder?>;

    public class SnoozeReminderHandler(IWebHostEnvironment env) : IRequestHandler<SnoozeReminderCommand, Reminder?>
    {
        public async Task<Reminder?> Handle(SnoozeReminderCommand request, CancellationToken cancellationToken)
        {
            var reminder = await DB.Queryable<Reminder>()
                .Join(DB.Collection<TodoItem>(), r => r.TodoId, t => t.ID, (r, t) => new { r, t })
                .Where(x => x.t.OwnerId == request.UserId && x.r.ID == request.Id)
                .Select(x => x.r).FirstOrDefaultAsync(cancellationToken);
            if (reminder == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }

            reminder.State = ReminderState.Snoozed;
            reminder.SnoozeUntil = env.IsDevelopment()
                ? DateTime.UtcNow.AddSeconds(request.Minutes)
                : DateTime.UtcNow.AddMinutes(request.Minutes);
            await reminder.SaveAsync(cancellation: cancellationToken);
            return reminder;
        }
    }

    public class SnoozeReminderCommandValidator : AbstractValidator<SnoozeReminderCommand>
    {
        public SnoozeReminderCommandValidator()
        {
            RuleFor(x => x.Minutes).NotNull().GreaterThan(0);
        }
    }
}