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
        app.MapPatch("/api/reminders/{id}/snooze", (string Id, SnoozeReminderCommand command, ISender sender) =>
            sender.Send(command with { Id = Id}));
    }
    public sealed record SnoozeReminderCommand(string Id, int minutes) : IRequest<Reminder?>;
    public class SnoozeReminderHandler : IRequestHandler<SnoozeReminderCommand, Reminder?>
    {
        public async Task<Reminder?> Handle(SnoozeReminderCommand request, CancellationToken cancellationToken)
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
                reminder.State = ReminderState.Snoozed;
                reminder.SnoozeUntil = DateTime.Now.AddSeconds(request.minutes);
                await reminder.SaveAsync();
                return reminder;
            }
        }
    }

    public class SnoozeReminderCommandValidator : AbstractValidator<SnoozeReminderCommand>
    {
        public SnoozeReminderCommandValidator()
        {
            RuleFor(x => x.minutes).NotNull().GreaterThan(0);
        }
    }
}