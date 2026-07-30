using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Reminders.DTOs;

namespace Todo.Bff.Features.Reminders.Application.Commands;

public sealed record DimissReminderCommand(string Id) : IRequest<ApiResult>;