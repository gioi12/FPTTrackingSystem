using DataTranferObjects.Login;
using Entities.Models;
using FPTTrackingSystem.Services.Authentication;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace FPTTrackingSystem.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly AuthUtils _authUtils;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthController(IAccountService accountService, AuthUtils authUtils,IRefreshTokenService refreshTokenService) 
        {
            _accountService = accountService;
            _authUtils = authUtils;
            _refreshTokenService = refreshTokenService;
        }
        [AllowAnonymous]
        [HttpPost("v1/auth/login")]
        public async Task<object> Login([FromBody] LoginDTO req)
        {
            var token = await _accountService.LoginAsync(req);
            var account = await _accountService.GetUser(req);
            string? deviceInfo = Request.Headers.TryGetValue("User-Agent", out var ua)
                ? ua.ToString()
                : "";
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(
                  account.User.Id,
                  device: deviceInfo,
                  ip: HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
              );
            var cookieValue = JsonSerializer.Serialize(new RefreshCookieModel
            {
                Token = refreshToken.Token,
                UserId = account.User.Id,
                Device = deviceInfo,
                Role = account.Role.Name
            });

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", cookieValue, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return ApiResponse<object>.Success(
             null,"Login Successfully",200);
           
        }
        [Authorize]
        [HttpPost("v1/auth/logout")]
        public async Task<object> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _refreshTokenService.RevokeRefreshTokenAsync(
                    refreshToken
                );
            }

            Response.Cookies.Delete("token");
            Response.Cookies.Delete("refresh_token");

            return ApiResponse<object>.Success(
                null, "Logout Successfully", 200);
        }

        [Authorize]
        [HttpGet("v1/auth/user-info")]
        public async Task<object> Info()
        {
            var info = await _authUtils.GetUserInfoFromCookie();
            if (info == null)
            {
                return ApiResponse<object>.Fail("Cannot get group",400);
             }
            return ApiResponse<object>.Success(
             info, "User information", 200);
        }
        [AllowAnonymous]
        [HttpPost("v1/auth/refreshToken")]
        public async Task<object> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var result = await _refreshTokenService.RotateRefreshTokenAsync(refreshToken);

            Response.Cookies.Append("token", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });
            var cookieValue = JsonSerializer.Serialize(result.RefreshToken);
            Response.Cookies.Append("refresh_token", cookieValue, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = result.RefreshTokenExpires
            });

            return ApiResponse<object>.Success(
                       null, "rotate successfully", 200);
        }
    }
}
