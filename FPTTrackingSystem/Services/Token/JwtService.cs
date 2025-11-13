using DataTranferObjects.Login;
using Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FPTTrackingSystem.Services.Token
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtService(IConfiguration config)
        {
            var jwtSettings = config.GetSection("JwtSettings");
            _secretKey = jwtSettings["SecretKey"]!;
            _issuer = jwtSettings["Issuer"]!;
            _audience = jwtSettings["Audience"]!;
            _expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);
        }

        public string GenerateToken(string userId, string role,string seId,string seName,string start,string end)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("semesterId", seId.ToString()),
                new Claim("semesterName", seName),
                new Claim("start_Time", start),
                new Claim("end_Time", end)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }


        public string? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public SemesterInfo GetSemesterFromToken(string token)
        {
            var principal = ValidateToken(token);
            return new SemesterInfo
            {
                SemesterId = principal?.FindFirst("semesterId")?.Value,
                SemesterName = principal?.FindFirst("semesterName")?.Value,
                Start_Time = principal?.FindFirst("start_Time")?.Value,
                End_Time = principal?.FindFirst("end_Time")?.Value,
                UserId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };
        }

        public bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            return jwtToken?.ValidTo < DateTime.UtcNow;
        }
    }
}
