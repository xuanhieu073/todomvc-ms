using Carter;
using Todo.Bff.Clients;
using Todo.Bff.DTOs;

namespace Todo.Bff.Modules
{
    public class BffTodosModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiGroup = app.MapGroup("/bff/todos");
            apiGroup.MapGet("/", async (TodoApiClient _apiClient) => {
                var response = await _apiClient.GetTodosAsync();
                return response;
            });

            apiGroup.MapGet("/{id}", async (TodoApiClient _apiClient, string Id) =>
            {
                var response = await _apiClient.GetTodoAsync(Id);
                return response;
            });

            apiGroup.MapPost("/", async (TodoApiClient _apiClient, CreateTodoRequest createTodoRequest) =>
            {
                var response = await _apiClient.CreateTodoAsync(createTodoRequest);
                return response;
            });

            apiGroup.MapPut("/{id}", async (TodoApiClient _apiClient, string Id, UpdateTodoRequest updateTodoRequest) =>
            {
                var response = await _apiClient.UpdateTodoAsync(Id, updateTodoRequest);
                return response;
            });

            apiGroup.MapPatch("{id}/toggle", async (TodoApiClient _apiClient, string Id) =>
            {
                var response = await _apiClient.ToggleIsComplete(Id);
                return response;
            });

            apiGroup.MapDelete("{id}", async (TodoApiClient _apiClient, string Id) =>
            {
                var response = await _apiClient.DeleteTodo(Id);
                return response;
            });
            apiGroup.MapDelete("/completed", async (TodoApiClient _apiClient) => await _apiClient.ClearCompleted());
        }
    }
}
