using Carter;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Reminders.Enpoints;

public class SnoozeReminderEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/reminders/{id}/snooze", (string id, SnoozeReminderCommand command, ISender sender) =>
            sender.Send(command with { Id = id }));
    }

    public sealed record SnoozeReminderCommand(string Id, int Minutes) : IRequest<Reminder?>;

    public class SnoozeReminderHandler : IRequestHandler<SnoozeReminderCommand, Reminder?>
    {
        public async Task<Reminder?> Handle(SnoozeReminderCommand request, CancellationToken cancellationToken)
        {
            var reminder = await DB.Find<Reminder>().OneAsync(request.Id, cancellationToken);
            if (reminder == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }
            else
            {
                reminder.State = ReminderState.Snoozed;
                reminder.SnoozeUntil = DateTime.UtcNow.AddSeconds(request.Minutes);
                await reminder.SaveAsync(cancellation: cancellationToken);
                return reminder;
            }
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