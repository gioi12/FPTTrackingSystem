using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Implements
{
    public class GroupRepository : IGroupRepository
    {
        private readonly FpttrackingSystemContext _context;
        public GroupRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<int> CountAsync(IQueryable<Group> query)
        {
            return await query.CountAsync();
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.Status)
                .Include(g => g.GroupUsers)
                    .ThenInclude(gu => gu.User)
/*                    .ThenInclude(u => u.Account)
                    .ThenInclude(a => a.Role)*/
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<GroupMentorDto>> GetExpiredGroupsByUserIdAsync(int userId)
        {
            var groups = await _context.GroupUsers
                .Include(gu => gu.Group)
                    .ThenInclude(g => g.GroupUsers)
                        .ThenInclude(gu2 => gu2.User)
                .Where(gu =>
                    gu.UserId == userId &&
                    gu.Group.ExpireDate.HasValue &&
                    gu.Group.ExpireDate < DateTime.UtcNow
                )
                .Select(gu => gu.Group)
                .Distinct()
                .ToListAsync();

            var result = groups.Select(g => new GroupMentorDto
            {
                Id = g.Id,
                GroupCode = g.Code,
                Name = g.Name,
                status = g.Status != null ? g.Status.Name : "active",
                IsExpired = g.ExpireDate != null && g.ExpireDate < DateTime.UtcNow,
                students = g.GroupUsers
                    .Where(gu => (gu.Role == StringEnum.Student || gu.Role == StringEnum.Secretary || gu.Role == StringEnum.Leader) && gu.IsActive)
                    .Select(s => new StudentGroupDTO
                    {
                        Id = s.User.Id,
                        Name = s.User.Fullname,
                        Email = s.User.Mail,
                        RollNumber = s.User.RollNumber
                    })
                    .ToList()
            }).ToList();

            return result;
        }

        public async System.Threading.Tasks.Task UpdateGroupAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Group>> GetAllAsync(Expression<Func<Group, bool>>? filter = null)
        {
            IQueryable<Group> query = _context.Groups.Include(g => g.GroupUsers);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<Group> UpdateAsync(Group updatedGroup)
        {
            var existingGroup = await _context.Groups
                .Include(g => g.GroupUsers)
                .FirstOrDefaultAsync(g => g.Id == updatedGroup.Id);

            if (existingGroup == null)
                throw new Exception($"Group with Id {updatedGroup.Id} not found.");

            existingGroup.ExpireDate = updatedGroup.ExpireDate;
            bool isUpdated = false;

            if (existingGroup.Name != updatedGroup.Name) { existingGroup.Name = updatedGroup.Name; isUpdated = true; }
            if (existingGroup.SemesterId != updatedGroup.SemesterId) { existingGroup.SemesterId = updatedGroup.SemesterId; isUpdated = true; }
            if (existingGroup.Profession != updatedGroup.Profession) { existingGroup.Profession = updatedGroup.Profession; isUpdated = true; }
            if (existingGroup.MajorId != updatedGroup.MajorId) { existingGroup.MajorId = updatedGroup.MajorId; isUpdated = true; }
            if (existingGroup.Description != updatedGroup.Description) { existingGroup.Description = updatedGroup.Description; isUpdated = true; }
            if (existingGroup.VietnameseTitle != updatedGroup.VietnameseTitle) { existingGroup.VietnameseTitle = updatedGroup.VietnameseTitle; isUpdated = true; }
            if (existingGroup.StatusId != updatedGroup.StatusId) { existingGroup.StatusId = updatedGroup.StatusId; isUpdated = true; }

            // Update GroupUsers
            foreach (var newGU in updatedGroup.GroupUsers)
            {
                var existingGU = existingGroup.GroupUsers.FirstOrDefault(gu => gu.UserId == newGU.UserId);
                if (existingGU == null)
                {
                    existingGroup.GroupUsers.Add(newGU);
                    isUpdated = true;
                }
                else
                {
                    if (existingGU.Role != newGU.Role) { existingGU.Role = newGU.Role; isUpdated = true; }
                    if (existingGU.IsActive != newGU.IsActive) { existingGU.IsActive = newGU.IsActive; isUpdated = true; }
                    if (existingGU.Status != newGU.Status) { existingGU.Status = newGU.Status; isUpdated = true; }
                    existingGU.UpdateAt = DateTime.Now;
                }
            }

            var removeUsers = existingGroup.GroupUsers
                .Where(gu => !updatedGroup.GroupUsers.Any(u => u.UserId == gu.UserId))
                .ToList();
            if (removeUsers.Any())
            {
                foreach (var gu in removeUsers)
                    existingGroup.GroupUsers.Remove(gu);
                isUpdated = true;
            }

            await _context.SaveChangesAsync();

            return existingGroup;
        }

        public IQueryable<GroupDto> GetGroupsQuery()
        {
            return _context.Groups
                .AsNoTracking()
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    CourseCode = g.Major != null ? g.Major.Code : "",
                    GroupCode = g.Code,
                    Term = g.Semester != null ? g.Semester.Name : "",
                    Major = g.Major != null ? g.Major.Name : "",
                    IsExpired = g.ExpireDate != null && g.ExpireDate < DateTime.UtcNow,
                    ExpireDate = g.ExpireDate,
                    StudentCount = g.GroupUsers.Count(gu =>
                        gu.Role == RoleEnum.Student.ToString() ||
                        gu.Role == "Leader" ||
                        gu.Role == "Secretary"),

                    Supervisor = g.GroupUsers
                        .Where(gu => gu.Role == "Supervisor" || gu.Role == "SupervisorHead")
                        .Select(gu => gu.User.Fullname),
                    SubmittedDocs = false
                });
        }

        public async Task<List<DashBoardGroupDto>> GetMajorGroupTotalsAsync()
        {
            return await _context.Majors
            .Select(m => new DashBoardGroupDto
            {
                name = m.Name ?? string.Empty,
                Total = m.MajorCategories.Count()
            })
            .ToListAsync();
        }

        public async Task<List<Group>> GetGroupsActiveSesmester()
        {
            var semester = await _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
            if (semester == null) throw new ValidationException("Not found sesmester currently active");
            return await _context.Groups.Include(x => x.Status)
                  .Where(x => x.SemesterId == semester.Id).ToListAsync();
        }

        public async Task<Group?> GetGroupWithMembersAsync(int groupId)
        {
            return await _context.Groups
                .Include(g => g.GroupUsers)
                    .ThenInclude(gu => gu.User)
                    .ThenInclude(u => u.Account)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<List<Milestone>> GetMilestonesByMajorAsync(int majorId)
        {
            return await _context.Milestones
                .Where(m => m.MajorId == majorId)
                .OrderBy(m => m.Deadline)
                .ToListAsync();
        }

        public async Task<GroupUser?> GetGroupUserAsync(int groupId, int userId)
        {
            return await _context.GroupUsers
                .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);
        }

        public async Task<List<Group>> GetGroupsByUserIdAsync(int userId)
        {
            return await _context.GroupUsers
        .Include(gu => gu.Group)
            .ThenInclude(g => g.GroupUsers)
                .ThenInclude(gu2 => gu2.User)
        .Where(gu => gu.UserId == userId && gu.IsActive && (gu.Group.ExpireDate == null || gu.Group.ExpireDate >= DateTime.UtcNow))
        .Select(gu => gu.Group)
        .ToListAsync();
        }

        public async Task<bool> UpdateRoleInGroupAsync(int groupId, int userId, string newRole)
        {
            var groupUser = await GetGroupUserAsync(groupId, userId);
            if (groupUser == null)
                return false;

            newRole = char.ToUpper(newRole[0]) + newRole.Substring(1).ToLower();

            if (newRole == "Secretary")
            {
                bool alreadyHasSecretary = await _context.GroupUsers
                    .AnyAsync(gu => gu.GroupId == groupId
                                 && gu.Role == "Secretary"
                                 && gu.UserId != userId);

                if (alreadyHasSecretary)
                    throw new InvalidOperationException("Nhóm này đã có một Secretary.");
            }

            groupUser.Role = newRole;
            _context.GroupUsers.Update(groupUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task CreateGroups(List<Group> groups)
        {
             await _context.Groups.AddRangeAsync(groups);
             await _context.SaveChangesAsync();
        }
    }
}
