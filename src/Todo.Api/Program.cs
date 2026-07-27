using Carter;
using FluentValidation;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Features.Reminders;
using Todo.Api.Features.Todos.DTOs;
using Todo.Api.Features.Todos.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddScoped<IValidator<CreateTodoRequest>, CreateTodoRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateTodoRequest>, UpdateTodoRequestValidator>();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));
//builder.Services.AddHostedService<RemindersScanners>();

await DB.InitAsync("TodoApp",
  MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapCarter();
app.Run();
