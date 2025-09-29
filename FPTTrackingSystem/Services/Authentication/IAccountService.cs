using DataTranferObjects.Login;

namespace FPTTrackingSystem.Services.Login
{
    public interface IAccountService
    {
        Task<string> LoginAsync(LoginDTO req);
    }
}
