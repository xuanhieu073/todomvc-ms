
using Todo.Bff.Features.Todos;
using Todo.Bff.Features.Todos.Endpoints;

namespace Todo.Bff.Clients;

public class TodoApiClient(HttpClient httpClient) : ApiClient(httpClient)
{
    public async Task<ApiResult> GetTodosAsync(string? filter, CancellationToken cancellationToken = default)
    {
        return filter switch
        {
            null => await GetAsync<List<TodoResponse>>("/api/todos", cancellationToken),
            _ => await GetAsync<List<TodoResponse>>($"/api/todos?filter={filter}", cancellationToken),
        };
    }

    public Task<ApiResult> GetTodoAsync(string id, CancellationToken ct = default)
        => GetAsync<TodoResponse>($"/api/todos/{id}", ct);

    public Task<ApiResult> CreateTodoAsync(CreateTodoRequest dto, CancellationToken ct = default)
        => SendAsync<TodoResponse>(HttpMethod.Post, "/api/todos", dto, ct);

    public Task<ApiResult> UpdateTodoAsync(string id, UpdateTodoRequest updateTodoRequest,
        CancellationToken cancellationToken = default)
        => SendAsync<TodoResponse>(HttpMethod.Put, $"/api/todos/{id}", updateTodoRequest, cancellationToken);

    public Task<ApiResult> ToggleIsCompleted(string id, CancellationToken cancellationToken = default)
        => SendAsync<TodoResponse>(HttpMethod.Patch, $"/api/todos/{id}/toggle", null, cancellationToken);

    public Task<ApiResult> DelteTodo(string id, CancellationToken cancellationToken = default)
        => DeleteAsync<string>($"/api/todos/{id}", cancellationToken);

    public Task<ApiResult> ClearCompleted(CancellationToken cancellationToken = default)
        => DeleteAsync<string>($"/api/todos/completed", cancellationToken);
}