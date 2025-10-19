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
            var account = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Users)
                    .ThenInclude(u => u.GroupUsers)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
                return null;

            var user = account.Users.FirstOrDefault();
            if (user == null)
                return null;

            var groupId = user.GroupUsers.FirstOrDefault()?.GroupId;

            var groupUser = user.GroupUsers.FirstOrDefault();
            var roleInGroup = groupUser?.Role;

            return new UserInfo
            {
                Id = user.Id,
                Name = user.Fullname,
                Role = account.Role.Name,
                RoleInGroup = roleInGroup,
                GroupId = groupId
            };
        }
    }
}
