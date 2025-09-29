using System.Security.Claims;

namespace FPTTrackingSystem.Services.Token
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string role);
        ClaimsPrincipal? ValidateToken(string token);
        string? GetUserIdFromToken(string token);
        bool IsTokenExpired(string token);
    }
}
