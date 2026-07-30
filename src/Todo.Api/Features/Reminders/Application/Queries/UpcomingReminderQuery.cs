using MediatR;
using Todo.Api.Features.Reminders;
using Todo.Api.Features.Todos.Entities;

public sealed record UpcomingReminderQuery(string within) : IRequest<List<TodoItem>>;