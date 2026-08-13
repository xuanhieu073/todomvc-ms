using System.Security.Cryptography;
using System.Text;
using Carter;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;

namespace Todo.Api.Features.Authentications.Endpoints;

public class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login",
            async (LoginCommand loginCommand, ISender sender) => await sender.Send(loginCommand));
    }

    public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

    public class LoginHandler(TokenService tokenService) : IRequestHandler<LoginCommand, LoginResponse>
    {
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await DB.Find<User>().Match(u => u.Email == request.Email).ExecuteFirstAsync(cancellationToken);
            if (user == null)
            {
                var error = new ValidationError("Email", $"The specified Email does not exist.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }

            if (request.Email != user.Email)
            {
                var error = new ValidationError("Email",
                    $"The specified Email does not match.");
                List<ValidationError> errors = [error];
                throw new NotFoundException(errors);
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) throw new UnauthorizedException("Invalid password");
            }

            return new LoginResponse() { Email = user.Email, AccessToken = tokenService.CreateToken(user) };
        }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}