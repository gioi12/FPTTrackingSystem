using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Login
{
    public class LoginDTO
    {
        public string userName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public sealed class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string? FullName { get; set; }
        public string? Roles { get; set; }
    }

    public sealed class TokensDTO
    {
        public string AccessToken { get; set; } = default!;
        public DateTime AccessTokenExpiresAt { get; set; }
    }

    public sealed class AuthResponseDTO
    {
        public UserDTO User { get; set; } = default!;
        public TokensDTO Tokens { get; set; } = default!;
    }
}
