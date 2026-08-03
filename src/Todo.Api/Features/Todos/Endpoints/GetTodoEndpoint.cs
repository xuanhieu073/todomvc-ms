using AutoMapper;
using Carter;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Todos.Endpoints
{
    public class GetTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/todos/{id}", async ([AsParameters] GetTodoQuery query, ISender sender) =>
                await sender.Send(query));
        }
    }

    public sealed record GetTodoQuery(string Id) : IRequest<TodoResponse>;

    public class GetTodoHandler(IMapper _mapper) : IRequestHandler<GetTodoQuery, TodoResponse>
    {
        public async Task<TodoResponse> Handle(GetTodoQuery request, CancellationToken cancellationToken)
        {
            var todo = await DB.Find<TodoItem>().OneAsync(request.Id);
            if (todo == null)
            {
                var error = new ValidationError("Id", $"The specified Todo ID does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }
            else
            {
                return _mapper.Map<TodoResponse>(todo);
            }
        }
    }
}
