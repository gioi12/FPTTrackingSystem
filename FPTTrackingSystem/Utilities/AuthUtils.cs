using Azure.Core;
using DataTranferObjects.Login;
using Entities.Models;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Services.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FPTTrackingSystem.Utilities
{
    public class AuthUtils
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly IJwtService _jwtService;
        public AuthUtils(IHttpContextAccessor httpContextAccessor, IAccountService accountService, IJwtService jwtService)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _jwtService = jwtService;
        }

        public Task<UserInfo?> GetUserInfoFromCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) throw new Exception("No context in read cookie");

            var token = httpContext.Request.Cookies["token"];
            if (string.IsNullOrEmpty(token)) throw new Exception("Not found information in cookie");
            var userId = _jwtService.GetUserIdFromToken(token);
            if (userId == null) throw new Exception("Not found user information in cookie");
            return _accountService.GetUserInfo(int.Parse(userId));
           
        }
    }
}

