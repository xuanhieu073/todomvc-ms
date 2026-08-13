namespace Todo.Api.Features.Authentications.Endpoints;

public record RegisterResponse(string Email, string AccessToken)
{
    public RegisterResponse() : this("", "")
    {
    }
}