
namespace Todo.Bff.Common
{
    public abstract class ApiResult
    {

        public int? StatusCode { get; init; }
        public string? ErrorMessage { get; init; }
        public string? ErrorCode { get; init; }
        public bool IsSuccess { get; init; }
        public abstract IResult ToHttpResponse();
    }
    public class ApiSucessResult<T> : ApiResult
    {
        public T? Data { get; init; }

        public static ApiSucessResult<T> Success(T data, int statusCode) =>
            new() { IsSuccess = true, Data = data, StatusCode = statusCode };

        public override IResult ToHttpResponse()
        {
            return Data is null
                    ? Results.NoContent()
                    : Results.Ok(Data);
        }
    }

    public class ApiErrorResult<T> : ApiResult
    {
        public T? Error { get; init; }

        public static ApiErrorResult<T> Failure(T? error, int statusCode, string message, string? errorCode = null) =>
            new() { Error = error, IsSuccess = false, StatusCode = statusCode, ErrorMessage = message, ErrorCode = errorCode };

        public override IResult ToHttpResponse()
        {
            return StatusCode switch
            {
                400 => Results.BadRequest(new { message = ErrorMessage, error = Error, code = ErrorCode }),
                404 => Results.NotFound(new { message = ErrorMessage }),
                401 => Results.Unauthorized(),
                403 => Results.Forbid(),
                429 => Results.Json(new { message = ErrorMessage }, statusCode: 429),
                _ => Results.Problem(
                        title: "Unkow Error",
                        detail: ErrorMessage,
                        statusCode: StatusCode ?? 500)
            };
        }
    }
}
