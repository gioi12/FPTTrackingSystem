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
        public async Task<Deliverable?> GetByMileIdAndActiveSenmester(int mileId)
        {
            var semester = await _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
            return await _context.Deliverables.Include(x => x.DeliveryItems).FirstOrDefaultAsync(x => x.MilestoneId == mileId && semester.Id == x.SemesterId);
        }

        public void UpdateDeliverable(Deliverable delivery)
        {
            _context.Deliverables.Update(delivery);
            _context.SaveChangesAsync();
        }
    }
}
