using DataTranferObjects.Login;
using Entities.Models;
using System.Text;

namespace DataAccessObjects.Login
{
    public class AuthenticationDAO
    {
        public AuthResponseDTO? Login(LoginDTO loginDto)
        {
            using var _context = new ProjectrackingsystemContext();

            // NOTE: hiện tại so sánh plaintext theo DB của bạn
            var user = _context.Users
                .FirstOrDefault(u => u.Username == loginDto.userName
                                  && u.Password == loginDto.Password);

            if (user is null) return null;

            var token = GenerateFakeToken(user);
            var expires = DateTime.UtcNow.AddHours(1);

            return new AuthResponseDTO
            {
                User = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.Name,
                    Roles = user.Role
                },
                Tokens = new TokensDTO
                {
                    AccessToken = token,
                    AccessTokenExpiresAt = expires
                }
            };
        }

        // FAKE token: không ký, không xác thực. Chỉ để chạy tạm.
        private static string GenerateFakeToken(User user)
        {
            var payload = $"{user.Id}:{user.Username}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            return $"FAKE.{base64}.{Guid.NewGuid():N}";
        }

        // Register(...) của bạn giữ nguyên
        public User Register(RegisterDTO registerDto)
        {
            using var _context = new ProjectrackingsystemContext();

            if (_context.Users.Any(u => u.Username == registerDto.userName))
                throw new Exception("Username already exists");

            var newUser = new User
            {
                Username = registerDto.userName,
                Password = registerDto.Password,
                Role = registerDto.Role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return newUser;
        }
    }
}
