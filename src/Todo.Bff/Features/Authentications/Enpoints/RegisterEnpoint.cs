using Carter;
using MediatR;
using Todo.Bff.Clients;

namespace Todo.Bff.Features.Authentications.Enpoints;

public class RegisterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/bff/auth/register",
            async (RegisterCommand registerCommand, ISender sender) =>
                (await sender.Send(registerCommand)).ToHttpResponse());
    }

    public sealed record RegisterCommand(string Email, string Password, string ConfirmPassword) : IRequest<ApiResult>;

    public class RegisterHandler(AuthApiClient apiClient) : IRequestHandler<RegisterCommand, ApiResult>
    {
        public async Task<ApiResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var response = await apiClient.Register(request, cancellationToken);
            return response;
        }
    }
}