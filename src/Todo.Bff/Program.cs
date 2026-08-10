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
builder.Services.AddHttpClient<ReminderApiClient>(c => c.BaseAddress = new Uri(builder.Configuration["TodoApi:BaseUrl"]!));
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddServiceBusClient("Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;");
});
// builder.Services.AddSingleton<SseStreamManager>();
builder.Services.AddSingleton<Queue1Broker>();
builder.Services.AddSingleton<Queue2Broker>();
builder.Services.AddHostedService<QueueConsumerWorker>();

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
