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
            return ApiResponse<string>.Success(
                token,"Login Successfully",200
                );
           
        }
    }
}
