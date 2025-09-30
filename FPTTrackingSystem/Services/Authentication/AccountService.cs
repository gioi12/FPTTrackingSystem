using DataTranferObjects.Login;
using Entities.Models;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Services.Token;
using FPTTrackingSystem.Wrappers;
using Repositories.Authentication;

namespace FPTTrackingSystem.Services.Authentication
{
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        private IJwtService _jwtService;

        public AccountService(IAccountRepository accountRepository, IJwtService jwtService)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
        }
        public async Task<string> LoginAsync(LoginDTO req)
        {
            Account? acc = await _accountRepository.LoginAsync(req);
            if (acc == null)
            {
                throw new DirectoryNotFoundException("login error");
            }
            return 
                _jwtService.GenerateToken(acc.Id.ToString(), acc.Role.Name);
        }
    }
}
