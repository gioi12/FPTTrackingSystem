using DataTranferObjects.Login;
using Entities.Models;

namespace FPTTrackingSystem.Services.Authentication
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CreateRefreshTokenAsync(int userId, string device, string ip);
        Task<RotateTokenResult> RotateRefreshTokenAsync(string rawRefreshToken);
        System.Threading.Tasks.Task RevokeRefreshTokenAsync(string rawToken);
    }
}
