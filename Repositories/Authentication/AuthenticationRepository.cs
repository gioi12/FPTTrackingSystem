using DataAccessObjects.Login;
using DataTranferObjects.Login;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Authentication
{
    public class AuthenticationRepository : IAuthentiicationRepository
    {

        private readonly AuthenticationDAO _authDao;

        public AuthenticationRepository(AuthenticationDAO authDao)
        {
            _authDao = authDao;
        }

        public User? Login(LoginDTO loginDto)
        {
            return _authDao.Login(loginDto);
        }

        public User Register(RegisterDTO registerDto)
        {
            return _authDao.Register(registerDto);
        }
    }
}
