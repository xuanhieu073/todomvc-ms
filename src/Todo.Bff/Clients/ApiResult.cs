namespace Todo.Bff.Clients;

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
        if (Data is null && StatusCode != null)
        {
            return Results.StatusCode((int)StatusCode);
        }

        return Results.Json(Data, statusCode: this.StatusCode);
    }
}
