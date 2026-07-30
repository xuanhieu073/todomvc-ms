using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Todos.Application.Commands;

public sealed record ToggleIsCompletedCommand(string Id) : IRequest<ApiResult>;