using Carter;
using Todo.Bff.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddHttpClient<TodoApiClient>(c => c.BaseAddress = new Uri(builder.Configuration["TodoApi:BaseUrl"]!));

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

app.UseHttpsRedirection();

app.MapCarter();
app.Run();
