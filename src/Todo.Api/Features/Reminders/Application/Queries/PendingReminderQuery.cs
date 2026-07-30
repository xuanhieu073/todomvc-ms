using MediatR;

namespace Todo.Api.Features.Reminders.Application.Queries;

public sealed record PendingReminderQuery(string state) : IRequest<List<ReminderDto>>;