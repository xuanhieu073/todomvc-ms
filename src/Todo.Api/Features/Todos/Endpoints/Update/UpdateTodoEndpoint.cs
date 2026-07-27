using Carter;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Todo.Api.Features.Todos.Application.Commands;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Endpoints.Update
{
    public class UpdateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/todos/{id}", async (IValidator<UpdateTodoRequest> _validator, string Id, UpdateTodoRequest updateTodoRequest, ISender sender) => {
                ValidationResult validationResults = _validator.Validate(updateTodoRequest);
                if (!validationResults.IsValid)
                {
                    return Results.BadRequest(validationResults.Errors);
                }
                else
                {
                    var command = new UpdateTodoCommand(Id, updateTodoRequest);
                    var result = await sender.Send(command);
                    return result switch
                    {
                        null => Results.NotFound("Invalid Id"),
                        _ => Results.Ok(result)
                    };
                }
            });
        }
    }
}
