using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.DTOs;
using Todo.Api.Entities;
using Todo.Api.Services;
using Todo.Api.Services.Interfaces;
using Todo.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IValidator<CreateToDoRequest>, TodoRequestValidator>();

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
