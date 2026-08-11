using AutoMapper;
using Carter;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Reminders.Enpoints;

public class UpdateFireAtEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/reminders/{id}/update-fire-at",
            ([AsParameters] UpdateFireAtCommand command, ISender sender) =>
                sender.Send(command));
    }

    public sealed record UpdateFireAtCommand(string Id) : IRequest<ReminderDto?>;

    public class UpdateFireAtHandler(IMapper mapper) : IRequestHandler<UpdateFireAtCommand, ReminderDto?>
    {
        public async Task<ReminderDto?> Handle(UpdateFireAtCommand request, CancellationToken cancellationToken)
        {
            var reminder = await DB.Find<Reminder>().OneAsync(request.Id, cancellationToken);
            if (reminder == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }

            reminder.FiredAt = DateTime.UtcNow;
            await reminder.SaveAsync(cancellation: cancellationToken);
            return mapper.Map<ReminderDto>(reminder);
        }
    }
}