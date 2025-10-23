using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories.Staff.Implements
{
    public class DeliverableRepository : IDeliverableRepository
    {
        private readonly FpttrackingSystemContext _context;
        public DeliverableRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Deliverable>> GetByCodeAndSemester(int code, int semesterId)
        {
           var list = await _context.Deliverables.Include(x=>x.DeliveryItems)
                .Include(x=>x.DeliverableGroups)
                .Where(x=>x.MajorId == code && x.SemesterId == semesterId && x.IsActive == true)
                .ToListAsync();
            return list;
        }

        public async Task<Deliverable?> GetById(int id)
        {
            return await _context.Deliverables.Include(x=>x.DeliveryItems).Include(x=>x.DeliverableGroups).FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<Deliverable?> GetByMileIdAndActiveSenmester(int mileId)
        {
            var semester = await _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
            return await _context.Deliverables.Include(x => x.DeliveryItems).FirstOrDefaultAsync(x => x.MilestoneId == mileId && semester.Id == x.SemesterId);
        }

        public async Task<DeliveryItem?> GetItemByItemId(int id)
        {
            return await _context.DeliveryItems
                .Include(x => x.Deliverable)
                    .ThenInclude(d => d.DeliverableGroups)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async System.Threading.Tasks.Task UpdateDeliverable(Deliverable delivery)
        {
            _context.Deliverables.Update(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DeliverableGroup>> GetDeliverableGroupsByGroupIdAsync(int groupId)
        {
            return await _context.DeliverableGroups
                .Include(dg => dg.Deliverable)
                .Where(dg => dg.GroupId == groupId)
                .ToListAsync();
        }

    }
}
