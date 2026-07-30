using Carter;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Features.Reminders;
using Todo.Api.Features.Todos.Middlewares;
using Todo.Api.Features.Todos.Validators;
using Todo.Api.MapingProfiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddHostedService<RemindersScanners>();

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
app.UseMiddleware<ValidationExceptionHandlingMiddleware>();
app.Run();