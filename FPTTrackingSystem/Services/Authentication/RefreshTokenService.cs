using DataTranferObjects.Login;
using Entities.Models;
using FPTTrackingSystem.Services.Token;
using Repositories.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FPTTrackingSystem.Services.Authentication
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRTRepository _rtRepository;
        private IJwtService _jwtService;
        private IAccountRepository _accountRepository;
        public RefreshTokenService(IRTRepository rtRepository,IJwtService jwtService, IAccountRepository accountRepository)
        {
            _rtRepository = rtRepository;
            _jwtService = jwtService;
            _accountRepository = accountRepository;
        }
        private string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
        public async Task<RefreshToken> CreateRefreshTokenAsync(int userId, string device, string ip)
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = HashToken(rawToken);

            var entity = new RefreshToken
            {
                UserId = userId,
                Token = hash,
                Device = device,
                IpAddress = ip,
                CreateAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            var isDone = await _rtRepository.CreateToken(entity);
            if(!isDone)
            {
                throw new Exception("Create refresh token failed");
            }
            entity.Token = rawToken;
            return entity;
        }
        private async Task<RefreshToken> CreateRTByRevoke(int userId, string device, string ip,DateTime expireDate)
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = HashToken(rawToken);

            var entity = new RefreshToken
            {
                UserId = userId,
                Token = hash,
                Device = device,
                IpAddress = ip,
                CreateAt = DateTime.UtcNow,
                ExpireAt = expireDate,
                IsRevoked = false
            };
            var isDone = await _rtRepository.CreateToken(entity);
            if (!isDone)
            {
                throw new Exception("Create refresh token failed");
            }
            entity.Token = rawToken;
            return entity;
        }
        public async System.Threading.Tasks.Task RevokeRefreshTokenAsync(string cookieValue)
        {
            var rawToken = "";
            int? userId = null;
            var role = "";
            if (!string.IsNullOrEmpty(cookieValue))
            {
                var data = JsonSerializer.Deserialize<RefreshCookieModel>(cookieValue);
                rawToken = data?.Token;
                userId = data?.UserId;
                role = data?.Role ?? "";
            }
            var hash = HashToken(rawToken);
            var token = await _rtRepository.FindByIdAndUserId((int)userId, hash);

            if (token != null)
            {
                var isDone =await _rtRepository.RevokeToken(token);
                if (!isDone)
                {
                    throw new Exception("Revoke refresh token failed");
                }
            }
        }

        public async Task<RotateTokenResult> RotateRefreshTokenAsync( string cookieValue)
        {
            var rawRefreshToken = "";
            int? userId = null;
            var role = "";
            if (!string.IsNullOrEmpty(cookieValue))
            {
                var data = JsonSerializer.Deserialize<RefreshCookieModel>(cookieValue);
                 rawRefreshToken = data?.Token;
                 userId = data?.UserId;
                 role = data?.Role ?? "";
            }
            var hash = HashToken(rawRefreshToken);
            if(userId == null)
            {
                throw new ValidationException("Invalid refresh token");
            }
            var oldToken = await _rtRepository.FindByIdAndUserId((int)userId, hash);

            if (oldToken == null)
                throw new ValidationException("Invalid refresh token");

            oldToken.IsRevoked = true;

            var isDone = _rtRepository.RevokeToken(oldToken);
            if (!isDone.Result)
            {
                throw new Exception("Rotate refresh token failed");
            }
            var newRefreshToken = await CreateRTByRevoke((int)oldToken.UserId, oldToken.Device, oldToken.IpAddress, (DateTime)oldToken.ExpireAt);
            var semester = await _accountRepository.GetSemesterByNow();
            var seId = semester?.Id.ToString() ?? "";
            var seName = semester?.Name ?? "No Active Semester";
            var startAt = semester?.StartAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            var endAt = semester?.EndAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            string token = _jwtService.GenerateToken(userId.ToString(), role, seId, seName, startAt, endAt);
            return new RotateTokenResult
            {
                AccessToken = token,
                RefreshToken = new RefreshCookieModel
                {
                    Token = newRefreshToken.Token,
                    UserId = (int)newRefreshToken.UserId,
                    Role = role
                },
                RefreshTokenExpires = (DateTime)oldToken.ExpireAt
            };
        }
    }
}
