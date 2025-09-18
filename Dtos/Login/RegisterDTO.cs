using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Login
{
    public class RegisterDTO
    {
        public string userName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; }
    }
}   
