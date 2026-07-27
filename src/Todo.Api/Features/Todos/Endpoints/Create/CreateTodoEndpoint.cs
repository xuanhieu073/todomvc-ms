using Carter;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Todo.Api.Features.Todos.Application.Commands;
using Todo.Api.Features.Todos.DTOs;

namespace Todo.Api.Features.Todos.Endpoints.Create
{
    public class CreateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/todos", async (IValidator<CreateTodoRequest> _validator, CreateTodoRequest createTodoRequest, ISender sender) =>
            {
                ValidationResult validationResults = _validator.Validate(createTodoRequest);
                if (!validationResults.IsValid)
                {
                    return Results.BadRequest(validationResults.Errors);
                }
                else
                { 
                    var command = new CreateTodoCommand(createTodoRequest);
                    var result = await sender.Send(command);
                    return Results.Ok(result);
                }
            });
        }
    }
}
