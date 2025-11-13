using DataTranferObjects.Login;
using System.Security.Claims;

namespace FPTTrackingSystem.Services.Token
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string role, string seId, string seName, string start, string end);
        ClaimsPrincipal? ValidateToken(string token);
        string? GetUserIdFromToken(string token);
        SemesterInfo GetSemesterFromToken(string token);
        bool IsTokenExpired(string token);
    }
}
