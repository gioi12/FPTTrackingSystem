using DataTranferObjects.Login;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Authentication;

namespace FPTTrackingSystem.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService; 

        public AuthController(IAccountService accountService) 
        {
            _accountService = accountService;
        }
        [AllowAnonymous]
        [HttpPost("v1/auth/login")]
        public async Task<object> Login([FromBody] LoginDTO req)
        {
            string token = await _accountService.LoginAsync(req);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,           
                Secure = true,             
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            };
            Response.Cookies.Append("token", token, cookieOptions);
            return ApiResponse<object>.Success(
             null,"Login Successfully",200);
           
        }
        [Authorize] 
        [HttpPost("v1/auth/logout")]
        public async Task<object> Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,           
                Secure = true,             
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(-1) 
            };

            Response.Cookies.Append("token", "", cookieOptions);

            return  ApiResponse<object>.Success(
             null, "Logout Successfully", 200);
        }

        [Authorize]
        [HttpGet("v1/user-info")]
        public async Task<object> Info()
        {

            return await _accountService.UserInfo(HttpContext.User);
        }
        
    }
}
