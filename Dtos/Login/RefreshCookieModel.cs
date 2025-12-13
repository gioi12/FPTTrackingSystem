using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Login
{
    public class RefreshCookieModel
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Device { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
