using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Response;
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

        public async Task<Deliverable?> GetById(int id)
        {
            return await _context.Deliverables.Include(x=>x.DeliveryItems).Include(x=>x.DeliverableGroups).FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<Deliverable?> GetByMileIdAndActiveSenmester(int mileId)
        {
            var semester = await _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
            return await _context.Deliverables.Include(x => x.DeliveryItems).FirstOrDefaultAsync(x => x.MilestoneId == mileId && semester.Id == x.SemesterId);
        }

        public async Task<DeliveryItem?> GetItemByItemId(int id, int groupId)
        {
            return await _context.DeliveryItems
                .AsNoTracking()
                .Select(x => new DeliveryItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Deliverable = x.Deliverable == null ? null : new Deliverable
                    {
                        Id = x.Deliverable.Id,
                        Name = x.Deliverable.Name,
                        Description = x.Deliverable.Description,
                        Deadline = x.Deliverable.Deadline,
                        DeliverableGroups = x.Deliverable.DeliverableGroups
                            .Where(dg => dg.GroupId == groupId)
                            .ToList()
                    }
                })
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

        public async Task<List<Deliverable>> GetByCodeAndSemester(int code, int semesterId)
        {
            var list = await _context.Deliverables.Include(x => x.DeliveryItems)
               .Include(x => x.DeliverableGroups)
               .Where(x => x.MajorId == code && x.SemesterId == semesterId && x.IsActive == true)
               .ToListAsync();
            return list;
        }

        public async Task<List<GroupDeliverableRes>> GetByCodeAndSemesterGroup(
         int code, int semesterId, int groupId)
            {
                return await _context.Deliverables
                    .Where(x => x.MajorId == code && x.SemesterId == semesterId)
                    .Select(x => new GroupDeliverableRes
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Deadline = x.Deadline,
                        Status = x.DeliverableGroups
                            .Where(g => g.GroupId == groupId)
                            .Select(g => g.Status)
                            .FirstOrDefault() ?? ProgressEnum.Unsubmitted,
                        DeliveryItems = x.DeliveryItems
                            .Select(di => new DeliverableItemRes
                            {
                                Id = di.Id,
                                Name = di.Name,
                                Description = di.Description
                            })
                            .ToList()
                    })
                    .ToListAsync();
            }

    }
}
