using DataTranferObjects.Login;
using System.Security.Claims;

namespace FPTTrackingSystem.Services.Login
{
    public interface IAccountService
    {
        Task<string> LoginAsync(LoginDTO req);
        Task<UserInfo> UserInfo(ClaimsPrincipal userClaims);
    }
}
