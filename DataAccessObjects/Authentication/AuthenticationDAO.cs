using DataTranferObjects.Login;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.Login
{
    public class AuthenticationDAO
    {
        public User? Login(LoginDTO loginDto)
        {
            using var _context = new ProjectrackingsystemContext();
            var user = _context.Users
                .FirstOrDefault(u => u.Username == loginDto.userName
                                     && u.Password == loginDto.Password);

            return user; 
        }

        public User Register(RegisterDTO registerDto)
        {
            using var _context = new ProjectrackingsystemContext();

            if (_context.Users.Any(u => u.Username == registerDto.userName))
            {
                throw new Exception("Username already exists");
            }

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
