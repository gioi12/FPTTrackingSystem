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
        public async Task<string> LoginAsync(LoginDTO req)
        {
            Account? acc = await _accountRepository.LoginAsync(req);
            if (acc == null)
            {
                throw new ValidationException("Not Found");
            }

            return 
                _jwtService.GenerateToken(acc.Id.ToString(), acc.Role.Name);
        }

        public Task<UserInfo?> UserInfo(ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new UnauthorizedAccessException();

            return _accountRepository.UserInfo(int.Parse(userIdClaim.Value));
        }
    }
}
