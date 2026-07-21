using Carter;
using Todo.Bff.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddHttpClient<TodoApiClient>(c => c.BaseAddress = new Uri(builder.Configuration["TodoApi:BaseUrl"]!));
var baseurl = builder.Configuration["TodoApi:BaseUrl"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapCarter();
app.Run();
