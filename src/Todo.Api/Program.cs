using Carter;
using FluentValidation;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Repository;
using Todo.Api.Features.Todos.Repository.Interfaces;
using Todo.Api.Features.Todos.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IValidator<CreateToDoRequest>, CreateTodoRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateTodoRequest>, UpdateTodoRequestValidator>();

await DB.InitAsync("TodoApp", new MongoClientSettings()
{
    Server = new MongoServerAddress("localhost", 27017),
    Credential = MongoCredential.CreateCredential("admin", "root", "example")
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapCarter();
app.Run();
