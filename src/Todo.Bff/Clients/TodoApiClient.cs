
using Todo.Bff.Features.Todos;
using Todo.Bff.Features.Todos.Endpoints;

namespace Todo.Bff.Clients;


class Error
{
    public List<TodoPropertiesError>? errors { get; set; }
}

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
    public Task<ApiResult> GetTodoAsync(string Id, CancellationToken ct = default)
    => GetAsync<TodoResponse>($"/api/todos/{Id}", ct);

    public Task<ApiResult> CreateTodoAsync(CreateTodoRequest dto, CancellationToken ct = default)
    => SendAsync<TodoResponse, Error>(HttpMethod.Post, "/api/todos", dto, ct);

    public Task<ApiResult> UpdateTodoAsync(string Id, UpdateTodoRequest updateTodoRequest, CancellationToken cancellationToken = default)
        => SendAsync<TodoResponse, Error>(HttpMethod.Put, $"/api/todos/{Id}", updateTodoRequest, cancellationToken);

    public Task<ApiResult> ToggleIsCompleted(string Id, CancellationToken cancellationToken = default)
        => SendAsync<TodoResponse, string>(HttpMethod.Patch, $"/api/todos/{Id}/toggle", null, cancellationToken);

    public Task<ApiResult> DelteTodo(string Id, CancellationToken cancellationToken = default)
        => DeleteAsync<string>($"/api/todos/{Id}", cancellationToken);

    public Task<ApiResult> ClearCompleted(CancellationToken cancellationToken = default)
       => DeleteAsync<string>($"/api/todos/completed", cancellationToken);
}