using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataTranferObjects.Staff.Group;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
namespace Repositories.Staff.Interfaces
{
    public interface IGroupRepository
    {
        public IQueryable<GroupDto> GetGroupsQuery();
        public Task<int> CountAsync(IQueryable<Group> query);
        public Task<Group> GetByIdAsync(int id);
        public Task<List<DashBoardGroupDto>> GetMajorGroupTotalsAsync();
        Task<List<Group>> GetAllAsync(Expression<Func<Group, bool>>? filter = null);
        public Task<List<Group>> GetGroupsActiveSesmester();
        System.Threading.Tasks.Task UpdateGroupAsync(Group group);
        public Task<Group?> GetGroupWithMembersAsync(int groupId);
        public Task<List<Milestone>> GetMilestonesByMajorAsync(int majorId);
        Task<List<Group>> GetGroupsByUserIdAsync(int userId);
        Task<bool> UpdateRoleInGroupAsync(int groupId, int userId, string newRole);
        System.Threading.Tasks.Task CreateGroups(List<Group> groups);
        Task<Group> UpdateAsync(Group updatedGroup);
        Task<List<GroupMentorDto>> GetExpiredGroupsByUserIdAsync(int userId);
    }
}
