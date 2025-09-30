using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.GroupRepository
{
    public interface IGroupRepository
    {
        public IQueryable<Entities.Models.Group> GetGroupsQuery();
        public Task<int> CountAsync(IQueryable<Entities.Models.Group> query);
        public Task<Entities.Models.Group> GetByIdAsync(int id);
    }
}
