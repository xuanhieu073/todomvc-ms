using AutoMapper;
using Carter;
using Carter.Request;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using Todo.Api.DTOs;
using Todo.Api.Entities;
using Todo.Api.Services;
using Todo.Api.Services.Interfaces;
using Todo.Api.Validators;

namespace Todo.Api.Modules;

public class TodoModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/todos");

        apiGroup.MapGet("", (ITodoService _todoService, HttpRequest req) =>
        {
            var filter = req.Query.As<string>("filter");
            return _todoService.Filter(filter);
        });

        apiGroup.MapGet("/{id}", async (ITodoService _todoService, string Id) => {
            var todo = await _todoService.GetTodoById(Id);
            if(todo == null)
            {
                return Results.NotFound("Invalid Id");
            } 
            else
            {
                return Results.Ok(todo);
            }
        });

        apiGroup.MapPost("", async (IValidator <CreateToDoRequest> _validator, ITodoService _todoService, CreateToDoRequest createTodoRequest) => {
            ValidationResult validationResults = _validator.Validate(createTodoRequest);
            if (!validationResults.IsValid)
            {
                return Results.BadRequest(validationResults.Errors);
            }
            else
            {
                var result = await _todoService.CreateTodo(createTodoRequest);
                return Results.Ok(result);
            }
        });

        apiGroup.MapPut("/{id}", async (IValidator<UpdateTodoRequest> _validator, ITodoService _todoService, string Id, UpdateTodoRequest updateTodoRequest) => {
            ValidationResult validationResults = _validator.Validate(updateTodoRequest);
            if(!validationResults.IsValid)
            {
                return Results.BadRequest(validationResults.Errors);
            }
            else
            {
                var result = await _todoService.UpdateTodo(Id, updateTodoRequest);
                return result switch
                {
                    null => Results.NotFound("Invalid Id"),
                    _ => Results.Ok(result)
                };
            }
        });

        apiGroup.MapPatch("/{id}/toggle", async (ITodoService _todoService, string Id) => {
            var result = await _todoService.ToggleIsCompleted(Id);
            return result switch
            {
                null => Results.NotFound("Invalid Id"),
                _ => Results.Ok(result)
            };
        });

        apiGroup.MapDelete("/{id}", async (ITodoService _todoService, string Id) =>
        {
            var result = await _todoService.DeleteTodo(Id);
            return result switch
            {
                null => Results.NotFound("Invalid Id"),
                false => Results.InternalServerError(),
                true => Results.Ok(),
            };
        });

        apiGroup.MapDelete("/completed", async (ITodoService _todoService) => {
            var deleteCount = await _todoService.ClearCompleted();
            return Results.Ok($"Deleted {deleteCount} todos");
        });
    }
}