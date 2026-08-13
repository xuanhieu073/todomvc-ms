using System.Net;
using System.Text;
using System.Text.Json;
using Todo.Bff.Common;

namespace Todo.Bff.Clients
{
    public class ApiClient(HttpClient httpClient)
    {
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        protected Task<ApiResult> DeleteAsync<TResponse>(
            string path, CancellationToken ct = default)
            => SendAsync<TResponse>(HttpMethod.Delete, path, null, ct);

        protected Task<ApiResult> GetAsync<TResponse>(
            string path, CancellationToken ct = default)
            => SendAsync<TResponse>(HttpMethod.Get, path, null, ct);

        protected async Task<ApiResult> SendAsync<TResponse>(
            HttpMethod method, string path, object? body, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(method, path);
            //request.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ1c2VyLXVuaXF1ZS1pZC0wMDEiLCJlbWFpbCI6InVzZXJuYW1lQG1haWxpbmF0b3IuY29tIiwicm9sZSI6IlVzZXIiLCJuYmYiOjE3ODY1MjcyMzMsImV4cCI6MTc4NjUzMDgzMywiaWF0IjoxNzg2NTI3MjMzLCJpc3MiOiJZb3VyQXBpSXNzdWVyIiwiYXVkIjoiWW91ckFwaUF1ZGllbmNlIn0.0tXWwvk5Y7lFupo0E2FDJ785Rcsn28MUGRwsM6j5Aj4");

            if (body is not null)
            {
                var json = JsonSerializer.Serialize(body, JsonOpts);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(request, cancellationToken);
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
                    var error = Deserialize<Error>(raw);
                    throw new ValidationException(error?.Errors!);
                }

                case HttpStatusCode.NotFound:
                {
                    var error = Deserialize<Error>(raw);
                    throw new NotFoundException(error?.Errors!);
                }

                case HttpStatusCode.Unauthorized:
                {
                    throw new UnauthorizedException("UnauthorizedException");
                }

                case HttpStatusCode.InternalServerError:
                    throw new Exception("Internal Server Error");
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:

                default:
                    throw new Exception("Unknow Error");
            }
        }

        private static TResponse? Deserialize<TResponse>(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return default;
            try
            {
                return JsonSerializer.Deserialize<TResponse>(raw, JsonOpts);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}
