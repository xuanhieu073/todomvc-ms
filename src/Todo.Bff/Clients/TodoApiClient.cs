using System.Net;
using System.Text;
using System.Text.Json;
using Todo.Bff.Features.Todos.Common;
using Todo.Bff.Features.Todos.DTOs;

namespace Todo.Bff.Clients
{
    public class TodoApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public TodoApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<ApiResult> GetTodosAsync(string? filter, CancellationToken cancellationToken = default)
        {
            return filter switch
           {
               null => await GetAsync<List<TodoDto>>("/api/todos", cancellationToken),
               _ => await GetAsync<List<TodoDto>>($"/api/todos?filter={filter}", cancellationToken),
           };
        }

        public Task<ApiResult> GetTodoAsync(string Id, CancellationToken ct = default)
        => GetAsync<TodoDto>($"/api/todos/{Id}", ct);

        public Task<ApiResult> CreateTodoAsync(CreateTodoRequest dto, CancellationToken ct = default)
        => CreateAsync<CreateTodoRequest, TodoDto, List<TodoPropertiesError>>("/api/todos", dto, ct);

        public Task<ApiResult> UpdateTodoAsync(string Id, UpdateTodoRequest updateTodoRequest, CancellationToken cancellationToken = default)
            => UpdateAsync<UpdateTodoRequest, TodoDto, List<TodoPropertiesError>>($"/api/todos/{Id}", updateTodoRequest, cancellationToken);

        public Task<ApiResult> ToggleIsCompleted(string Id, CancellationToken cancellationToken = default)
            => SendAsync<TodoDto, string>(HttpMethod.Patch, $"/api/todos/{Id}/toggle", null, cancellationToken);

        public Task<ApiResult> DelteTodo(string Id, CancellationToken cancellationToken = default)
            => DeleteAsync<string>($"/api/todos/{Id}", cancellationToken);

        public Task<ApiResult> ClearCompleted(CancellationToken cancellationToken = default)
           => DeleteAsync<string>($"/api/todos/completed", cancellationToken);


        private Task<ApiResult> CreateAsync<TRequest, TResponse, TError>(
            string path, TRequest body, CancellationToken ct = default)
            => SendAsync<TResponse, TError>(HttpMethod.Post, path, body, ct);

        private Task<ApiResult> UpdateAsync<TRequest, TResponse, TError>(
            string path, TRequest body, CancellationToken ct = default)
            => SendAsync<TResponse, TError>(HttpMethod.Put, path, body, ct);

        private Task<ApiResult> DeleteAsync<TResponse>(
            string path, CancellationToken ct = default)
            => SendAsync<TResponse, string>(HttpMethod.Delete, path, null, ct);

        private Task<ApiResult> GetAsync<TResponse>(
            string path, CancellationToken ct = default)
            => SendAsync<TResponse, string>(HttpMethod.Get, path, null, ct);

        private async Task<ApiResult> SendAsync<TResponse, TError>(
            HttpMethod method, string path, object? body, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(method, path);

            if (body is not null)
            {
                var json = JsonSerializer.Serialize(body, JsonOpts);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response;
            try
            {
                response = await _http.SendAsync(request, cancellationToken);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new Exception($"Request timeout.", ex);
            }

            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    {
                        var data = Deserialize<TResponse>(raw);
                        return ApiSucessResult<TResponse>.Success(data!, (int)response.StatusCode);
                    }

                case HttpStatusCode.NoContent:
                    return ApiSucessResult<TResponse>.Success(default!, (int)response.StatusCode);

                case HttpStatusCode.BadRequest:
                    {
                        var error = Deserialize<TError>(raw);
                        return ApiErrorResult<TError>.Failure(error, 400, "Bad request", "VALIDATION_ERROR");
                    }

                case HttpStatusCode.NotFound:
                    {
                        var errorMessage = Deserialize<string>(raw);
                        return ApiErrorResult<string>.Failure(errorMessage, 404, errorMessage ?? "", "NOT_FOUND");
                    }
                case HttpStatusCode.InternalServerError:
                    return ApiErrorResult<string>.Failure("Internal Server Error",500, "Internal Server Error", "INTERNAL_SERVER_ERROR");
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:

                default:
                    return ApiErrorResult<string>.Failure("Unknow Error",(int)response.StatusCode, "Unknow Error", "UNKNOWN");
            }
        }

        private static TResponse? Deserialize<TResponse>(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return default;
            try { return JsonSerializer.Deserialize<TResponse>(raw, JsonOpts); }
            catch (JsonException) { return default; }
        }
    }
}
