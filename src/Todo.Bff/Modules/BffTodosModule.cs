using Carter;
using Carter.Request;
using Todo.Bff.Clients;
using Todo.Bff.DTOs;

namespace Todo.Bff.Modules
{
    public class BffTodosModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiGroup = app.MapGroup("/bff/todos");
            apiGroup.MapGet("/", async (TodoApiClient _apiClient, HttpRequest req) => {
                var filter = req.Query.As<string>("filter");
                var response = await _apiClient.GetTodosAsync(filter);
                return response.ToHttpResponse();
            });

            apiGroup.MapGet("/{id}", async (TodoApiClient _apiClient, string Id) =>
            {
                var response = await _apiClient.GetTodoAsync(Id);
                return response.ToHttpResponse();
            });

            apiGroup.MapPost("/", async (TodoApiClient _apiClient, CreateTodoRequest createTodoRequest) =>
            {
                var response = await _apiClient.CreateTodoAsync(createTodoRequest);
                return response.ToHttpResponse();
            });

            apiGroup.MapPut("/{id}", async (TodoApiClient _apiClient, string Id, UpdateTodoRequest updateTodoRequest) =>
            {
                var response = await _apiClient.UpdateTodoAsync(Id, updateTodoRequest);
                return response.ToHttpResponse();
            });

            apiGroup.MapPatch("{id}/toggle", async (TodoApiClient _apiClient, string Id) =>
            {
                var response = await _apiClient.ToggleIsCompleted(Id);
                return response.ToHttpResponse();
            });

            apiGroup.MapDelete("{id}", async (TodoApiClient _apiClient, string Id) =>
            {
                var response = await _apiClient.DelteTodo(Id);
                return response.ToHttpResponse();
            });

            apiGroup.MapDelete("/completed", async (TodoApiClient _apiClient) => {
                var response = await _apiClient.ClearCompleted();
                return response.ToHttpResponse();
            });
        }
    }
}
