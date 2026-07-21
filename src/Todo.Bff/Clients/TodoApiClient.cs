using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using Todo.Bff.DTOs;

using ApiResponse = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Ok<Todo.Bff.DTOs.TodoDto>,
    Microsoft.AspNetCore.Http.HttpResults.Ok<string>,
    Microsoft.AspNetCore.Http.HttpResults.Ok,
    Microsoft.AspNetCore.Http.HttpResults.NotFound<string>,
    Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>;

namespace Todo.Bff.Clients
{
    public class TodoApiClient
    {
        private readonly HttpClient _httpClient;

        public TodoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TodoDto>> GetTodosAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<List<TodoDto>>("api/todos", cancellationToken);
            return response ?? new List<TodoDto>();
        }

        public async Task<ApiResponse> GetTodoAsync(string Id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/todos/{Id}", cancellationToken);
            return await ReturnResult(response, cancellationToken);
        }

        public async Task<ApiResponse> CreateTodoAsync(CreateTodoRequest createTodoRequest, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/todos", createTodoRequest, cancellationToken);
            return await ReturnResult(response, cancellationToken);
        }

        public async Task<ApiResponse> UpdateTodoAsync(string Id, UpdateTodoRequest updateTodoRequest, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/todos/{Id}", updateTodoRequest, cancellationToken);
            return await ReturnResult(response, cancellationToken);
        }

        public async Task<ApiResponse> ToggleIsComplete(string Id, CancellationToken cancellationToken = default) 
        {
            var response = await _httpClient.PatchAsync($"/api/todos/{Id}/toggle",null, cancellationToken);
            return await ReturnResult(response, cancellationToken);
        }

        public async Task<ApiResponse> DeleteTodo(string Id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"/api/todos/{Id}", cancellationToken);
            return await ReturnResult(response, cancellationToken);
        }

        public async Task<ApiResponse> ClearCompleted(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"/api/todos/completed", cancellationToken);
            return await ReturnResult(response, cancellationToken);
        }

        public async Task<ApiResponse> ReturnResult(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.ContentLength != 0)
                {
                    try
                    {
                        var todoDto = await response.Content.ReadFromJsonAsync<TodoDto>(cancellationToken);
                        return TypedResults.Ok(todoDto);
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        string rawText = await response.Content.ReadAsStringAsync(cancellationToken);
                        return TypedResults.Ok(rawText);
                    }
                }
                return TypedResults.Ok();
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return TypedResults.NotFound("Invalid Id");
            }
            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return TypedResults.BadRequest(errorMessage);
        }
    }
}
