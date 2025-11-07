using DataTranferObjects.Login;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Repositories.Authentication
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FpttrackingSystemContext _context;

        public AccountRepository(FpttrackingSystemContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Account?> LoginAsync(LoginDTO req)
        {
            return await _context.Accounts
                .Include(x=>x.Role)
                .FirstOrDefaultAsync(x => x.Username == req.UserName && x.Password == req.Password);
        }

        public async Task<List<Account>> GetAllAsync(Expression<Func<Account, bool>> predicate)
        {
            return await _context.Accounts.Where(predicate).ToListAsync();
        }


        public async Task<UserInfo?> UserInfo(int id)
        {
            var semesterIdCookie = _httpContextAccessor.HttpContext?.Request.Cookies["semesterId"];
            int? currentSemesterId = null;

            if (!string.IsNullOrWhiteSpace(semesterIdCookie) && int.TryParse(semesterIdCookie, out int semesterIdValue))
            {
                currentSemesterId = semesterIdValue;
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Users)
                    .ThenInclude(u => u.GroupUsers)
                        .ThenInclude(gu => gu.Group)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
                return null;

            var user = account.Users.FirstOrDefault();
            if (user == null)
                return null;

            var groupIds = new List<int>();

            if (currentSemesterId.HasValue)
            {
                groupIds = user.GroupUsers
                    .Where(gu => gu.Group != null && gu.Group.SemesterId == currentSemesterId.Value)
                    .Select(gu => gu.GroupId)
                    .ToList();
            }
            else
            {
                groupIds = user.GroupUsers
                    .Select(gu => gu.GroupId)
                    .ToList();
            }

            var groupUser = user.GroupUsers.FirstOrDefault();
            var roleInGroup = groupUser?.Role;

            return new UserInfo
            {
                Id = user.Id,
                Name = user.Fullname,
                Role = account.Role.Name,
                RoleInGroup = roleInGroup,
                Groups = groupIds.Any() ? groupIds : new List<int>() 
            };
        }

        public async Task<Semester?> GetSemesterByNow()
        {
            return await _context.Semesters.FirstOrDefaultAsync(s => s.IsActive == true);
        }

        public async Task<List<Account>> CreateUsers(List<Account> accounts)
        {
            if (accounts == null || accounts.Count == 0)
                throw new Exception("Account list cannot be null or empty");

            await _context.Accounts.AddRangeAsync(accounts);
            await _context.SaveChangesAsync();

            return accounts;
        }
    }
}
