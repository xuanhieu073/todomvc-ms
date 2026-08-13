using System.Security.Claims;

namespace Todo.Api.Common;

public static class ClaimsPrincipleExtensions
{
    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
    }
}