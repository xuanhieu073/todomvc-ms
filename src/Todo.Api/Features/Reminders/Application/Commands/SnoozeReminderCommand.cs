using MediatR;
using Todo.Api.Features.Reminders;

namespace Todo.Api.Features.Reminders.Application.Commands;

public sealed record SnoozeReminderCommand(string Id) : IRequest<Reminder?>;