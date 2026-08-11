using Carter;
using Microsoft.Extensions.Azure;
using Todo.Bff.Clients;
using Todo.Bff.Common;
using Todo.Bff.Features.Reminders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddHttpClient<TodoApiClient>(c => c.BaseAddress = new Uri(builder.Configuration["TodoApi:BaseUrl"]!));
builder.Services.AddHttpClient<ReminderApiClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["TodoApi:BaseUrl"]!));
builder.Services.AddHttpClient<StatisticApiClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["TodoApi:BaseUrl"]!));
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("ASBConnectionString"));
});

var domain = builder.Configuration["Mailgun:Domain"];
var apiKey = builder.Configuration["Mailgun:ApiKey"];
var fromEmail = builder.Configuration["Mailgun:DefaultFromEmail"];
var emailBuilder = builder.Services.AddFluentEmail(fromEmail);
if (builder.Environment.IsDevelopment())
{
    emailBuilder.AddSmtpSender("127.0.0.1", 2525);
}
else
{
    emailBuilder.AddMailGunSender(domain, apiKey);
}

builder.Services.AddSingleton<NotificationBroker>();
builder.Services.AddHostedService<NotificationConsumerWorker>();
builder.Services.AddHostedService<EmailConsumerWorker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAngularApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapCarter();
app.Run();
