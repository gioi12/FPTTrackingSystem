using DataTranferObjects.Login;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Authentication
{
    public class AccountRepository : IAccountRepository
    {

        private readonly FpttrackingSystemContext _context;

        public AccountRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<Account?> LoginAsync(LoginDTO req)
        {
            return await _context.Accounts
                .Include(x=>x.Role)
                .FirstOrDefaultAsync(x => x.Username == req.UserName && x.Password == req.Password);
        }

        public async Task<UserInfo?> UserInfo(int id)
        {
            var user =  await _context.Accounts.Include(x => x.Users)
                .Where(x => x.Id == id)
                .Select(x => new UserInfo
                {
                    Id = x.Users.FirstOrDefault().Id,
                    Name = x.Users.FirstOrDefault().Fullname,
                    Role = x.Role.Name,
                }).FirstOrDefaultAsync();
            return user;
        }
    }
}
