using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Authentications.Enpoints;

public class LoginEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapPost("/bff/auth/login",
      async (LoginCommand loginCommand, ISender sender) => (await sender.Send(loginCommand)).ToHttpResponse());
  }

  public sealed record LoginCommand(string Email, string Password) : IRequest<ApiResult>;

  public class LoginHandler(AuthApiClient apiClient) : IRequestHandler<LoginCommand, ApiResult>
  {
    public async Task<ApiResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
      var response = await apiClient.Login(request, cancellationToken);
      return response;
    }
  }
}