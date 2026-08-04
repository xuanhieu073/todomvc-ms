using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Reminders.Enpoints;

public class DimissReminderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/bff/reminders/{id}/dimiss", async (string Id, ISender sender) =>
        {
            var command = new DimissReminderCommand(Id);
            var response = await sender.Send(command);
            return response.ToHttpResponse();
        });
    }
}

public sealed record DimissReminderCommand(string Id) : IRequest<ApiResult>;

public class DimissReminderHandler(ReminderApiClient _apiClient) : IRequestHandler<DimissReminderCommand, ApiResult>
{
    public Task<ApiResult> Handle(DimissReminderCommand request, CancellationToken cancellationToken)
    {
        return _apiClient.DimissReminder(request.Id);
    }
}