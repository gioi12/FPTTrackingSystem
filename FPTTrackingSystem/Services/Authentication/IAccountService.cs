using DataTranferObjects.Login;
using Entities.Models;
using System.Security.Claims;

namespace FPTTrackingSystem.Services.Login
{
    public interface IAccountService
    {
        Task<string> LoginAsync(LoginDTO req);
        Task<UserInfo?> GetUserInfo(SemesterInfo info);
    }
}
