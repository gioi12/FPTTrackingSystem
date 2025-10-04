using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTranferObjects.Staff.Group;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
namespace Repositories.Staff
{
    public interface IGroupRepository
    {
        public IQueryable<Group> GetGroupsQuery();
        public Task<int> CountAsync(IQueryable<Group> query);
        public Task<Group> GetByIdAsync(int id);
        public Task<List<DashBoardGroupDto>> GetMajorGroupTotalsAsync();

        public Task<List<Group>> GetGroupsActiveSesmester();
        public Task<Group?> GetGroupWithMembersAsync(int groupId);
        public Task<List<Milestone>> GetMilestonesByMajorAsync(int majorId);
    }
}
