using DataTranferObjects.Login;
using Entities.Models;
using System.Security.Claims;

namespace FPTTrackingSystem.Services.Login
{
    public interface IAccountService
    {
        Task<(string token, Semester? semester)> LoginAsync(LoginDTO req);
        Task<UserInfo?> GetUserInfo(int id);
    }
}
