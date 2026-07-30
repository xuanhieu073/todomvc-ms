using MediatR;
using Todo.Api.Features.Reminders;

public sealed record DimissReminderCommand(string Id) : IRequest<Reminder?>;