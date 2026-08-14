using Carter;
using Microsoft.Extensions.Azure;
using Todo.Bff.Clients;
using Todo.Bff.Common;
using Todo.Bff.Features.Reminders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddSingleton<CircuitBreakerHandler>();
builder.Services.AddTransient<ClientAuthDelegatingHandler>();
builder.Services.AddTransient<RetryHandler>();
builder.Services.AddTodoApiClient<TodoApiClient>();
builder.Services.AddTodoApiClient<ReminderApiClient>();
builder.Services.AddTodoApiClient<StatisticApiClient>();
builder.Services.AddTodoApiClient<AuthApiClient>(useAuth: false);

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("ASBConnectionString"));
});

var fromEmail = builder.Configuration.GetValue<string>("Mailgun:DefaultFromEmail")
                ?? throw new InvalidOperationException("Mailgun:DefaultFromEmail is missing in configuration.");
var emailBuilder = builder.Services.AddFluentEmail(fromEmail);
if (builder.Environment.IsDevelopment())
{
    emailBuilder.AddSmtpSender("127.0.0.1", 2525);
}
else
{
    var domain = builder.Configuration.GetValue<string>("Mailgun:Domain")
                 ?? throw new InvalidOperationException("Mailgun:Domain is missing in configuration.");
    var apiKey = builder.Configuration.GetValue<string>("Mailgun:ApiKey")
                 ?? throw new InvalidOperationException("Mailgun:ApiKey is missing in configuration.");
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
