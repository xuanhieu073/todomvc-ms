using MediatR;
using Todo.Bff.Clients;
using Todo.Bff.Features.Reminders.DTOs;

namespace Todo.Bff.Features.Reminders.Application.Commands;

public sealed record UpdateFireAtCommand(string Id) : IRequest<ApiResult>;