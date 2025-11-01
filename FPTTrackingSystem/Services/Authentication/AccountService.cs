using DataTranferObjects.Login;
using Entities.Models;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Services.Token;
using FPTTrackingSystem.Wrappers;
using Repositories.Authentication;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        public async Task<(string token, Semester? semester)> LoginAsync(LoginDTO req)
        {
            Account? acc = await _accountRepository.LoginAsync(req);
            if (acc == null)
                throw new ValidationException("Invalid username or password");

            var semester = await _accountRepository.GetSemesterByNow();

            string token = _jwtService.GenerateToken(acc.Id.ToString(), acc.Role.Name);
            return (token, semester);
        }


        public Task<UserInfo?> GetUserInfo(int id)
        {
            return _accountRepository.UserInfo(id);
        }
    }
}
