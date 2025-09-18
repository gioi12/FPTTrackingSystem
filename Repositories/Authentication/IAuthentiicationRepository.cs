using DataTranferObjects.Login;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Authentication
{
    public interface IAuthentiicationRepository
    {
        public User? Login(LoginDTO loginDto);
        public User Register(RegisterDTO registerDto);
    }
}
