using MongoDB.Entities;

namespace Todo.Api.Features.Authentications;

public class User : Entity
{
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}