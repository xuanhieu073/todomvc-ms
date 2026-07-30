using MediatR;
using Todo.Bff.Features.Reminders.DTOs;

namespace Todo.Bff.Features.Reminders.Application.Queries;

public sealed record RemindersStreamQuery(ReminderState state) : IRequest<List<PendingReminderDto>>;