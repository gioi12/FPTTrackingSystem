using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Login
{
    public class RotateTokenResult
    {
        public string AccessToken { get; set; } = null!;
        public RefreshCookieModel RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpires { get; set; }
    }
}
