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
            return await _context.Accounts.Include(u => u.User).Where(predicate).ToListAsync();

        }


        public async Task<UserInfo?> UserInfo(SemesterInfo info)
        {
            int? currentSemesterId = null;

            if (!string.IsNullOrWhiteSpace(info.SemesterId) && int.TryParse(info.SemesterId, out int semesterIdValue))
            {
                currentSemesterId = semesterIdValue;
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.User)
                    .ThenInclude(u => u.Campus)
                .Include(a => a.User)
                   .ThenInclude(u => u.GroupUsers)
                        .ThenInclude(gu => gu.Group)
                            .ThenInclude(sem => sem.Semester)
                .FirstOrDefaultAsync(a => a.Id == int.Parse(info.UserId));

            if (account == null)
                return null;

            var user = account.User;
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

            var semesterId = user.GroupUsers
                    .Where(gu => gu.Group != null)
                    .Select(gu => gu.Group.SemesterId)
                    .FirstOrDefault();

            GroupUser? groupUser = null;

            if (currentSemesterId.HasValue)
            {
                groupUser = user.GroupUsers
                    .Where(gu => gu.Group != null && gu.Group.SemesterId == currentSemesterId.Value)
                    .FirstOrDefault();
            }
            else
            {
                groupUser = user.GroupUsers.FirstOrDefault();
            }

            var roleInGroup = groupUser?.Role;

            // lay group
            var groups = user.GroupUsers
                .Where(gu => gu.Group != null)
                .Select(gu => new GroupInfo
                {
                    Id = gu.Group.Id,
                    Name = gu.Group.Name,
                    Code = gu.Group.Code,
                    IsExpired = CheckNow(gu.Group.ExpireDate),
                    SemesterId = (int)gu.Group.SemesterId,
                    SesesterName = gu.Group.Semester != null ? gu.Group.Semester.Name : string.Empty
                })
                .ToList();

            return new UserInfo
            {
                Id = user.Id,
                SemesterId = semesterId,
                Name = user.Fullname,
                Email = user.Mail,
                RollNumber = user.RollNumber,
                ExpireDate = groupUser?.Group?.ExpireDate,
                Role = account.Role.Name,
                RoleInGroup = roleInGroup,
                CampusId = user.CampusId,
                Groups = groupIds.Any() ? groupIds : new List<int>(),
                GroupsInfo = groups
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

        public async System.Threading.Tasks.Task UpdateAsync(Account account)
        {
            var existingAccount = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == account.Id);

            if (existingAccount == null) return;

            existingAccount.Password = account.Password;
            existingAccount.RoleId = account.RoleId;

            var existingUser = existingAccount.User;
            var newUser = account.User;

            if (existingUser != null && newUser != null)
            {
                existingUser.RollNumber = newUser.RollNumber;
                existingUser.Fullname = newUser.Fullname;
                existingUser.Dob = newUser.Dob;
                existingUser.Gender = newUser.Gender;
                existingUser.Mail = newUser.Mail;
                existingUser.Phone = newUser.Phone;
                existingUser.MajorId = newUser.MajorId;
                existingUser.CampusId = newUser.CampusId;
                existingUser.CapstoneProject = newUser.CapstoneProject;
                existingUser.Address = newUser.Address;
                existingUser.StatusId = newUser.StatusId;
            }

            _context.Accounts.Update(existingAccount);
            await _context.SaveChangesAsync();
        }
        public bool CheckNow(DateTime? dateCheck)
        {
            if (dateCheck < DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
