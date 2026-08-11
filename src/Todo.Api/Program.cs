using Carter;
using FluentValidation;
using Microsoft.Extensions.Azure;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Common;
using Todo.Api.Features.Reminders;

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
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("ASBConnectionString"));
});
await DB.InitAsync("TodoApp",
    MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ValidationExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapCarter();
app.Run();