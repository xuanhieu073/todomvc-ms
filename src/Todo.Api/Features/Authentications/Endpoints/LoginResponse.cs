namespace Todo.Api.Features.Authentications.Endpoints;

public record LoginResponse(string Email, string AccessToken)
{
    public LoginResponse() : this("", "")
    {
    }
}