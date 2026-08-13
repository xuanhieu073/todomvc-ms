using System.Security.Cryptography;
using System.Text;
using Carter;
using FluentValidation;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Common;
using ValidationException = Todo.Api.Common.ValidationException;

namespace Todo.Api.Features.Authentications.Endpoints;

public class RegisterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register",
            async (RegisterCommand registerCommand, ISender sender) => await sender.Send(registerCommand));
    }

    public record RegisterCommand(string Email, string Password, string ConfirmPassword) : IRequest<RegisterResponse>;

    public class RegisterHandler(TokenService tokenService) : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existedUser = await DB.Find<User>().Match(u => u.Email == request.Email)
                .ExecuteFirstAsync(cancellationToken);
            if (existedUser != null)
            {
                var error = new ValidationError("Email", $"The specified Email does exist.");
                List<ValidationError> errors = [error];
                throw new ValidationException(errors);
            }

            using var hmac = new HMACSHA512();
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                PasswordSalt = hmac.Key
            };
            await newUser.SaveAsync(cancellation: cancellationToken);
            return new RegisterResponse() { Email = newUser.Email, AccessToken = tokenService.CreateToken(newUser) };
        }
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
        }
    }
}