using Azure;
using DataTranferObjects.Login;
using Entities.Models;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Services.Token;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
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
                throw new ValidationException("Invalid username or password");

            var semester = await _accountRepository.GetSemesterByNow();
            var seId = semester?.Id.ToString() ?? "";
            var seName = semester?.Name ?? "No Active Semester";
            var startAt = semester?.StartAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            var endAt = semester?.EndAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            string token = _jwtService.GenerateToken(acc.Id.ToString(), acc.Role.Name,seId,seName,startAt,endAt);
            return token;
        }


        public Task<UserInfo?> GetUserInfo(SemesterInfo info)
        {
            return _accountRepository.UserInfo(info);
        }
    }
}
