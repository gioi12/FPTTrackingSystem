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
    public class MilestoneRepository : IMilestoneRepository
    {
        private readonly FpttrackingSystemContext _context;

        public MilestoneRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Milestone>> NewMilestontes(List<Milestone> list, int majorId)
        {
            await _context.Milestones.AddRangeAsync(list);
            await _context.SaveChangesAsync();
            return await _context.Milestones
                .Include(x => x.Major)
                .Include(x => x.CreateByNavigation)
                .Include(x => x.MilestoneItems)
                .Where(x => x.IsActive == true && x.MajorId == majorId)
                .ToListAsync();
        }

        public async Task<List<Milestone>> DeleteMilestone(int id)
        {
            var milestone = await _context.Milestones
                         .Include(m => m.Deliverables)
                         .FirstOrDefaultAsync(x => x.Id == id);

            var semester = await _context.Semesters.Where(x => x.IsActive == true).FirstOrDefaultAsync();

            if (milestone != null)
            {
                foreach (var deli in milestone.Deliverables.Where(x => x.SemesterId == semester.Id))
                {
                    deli.IsActive = false;
                }
                milestone.IsActive = false;
                _context.Milestones.Update(milestone);
                await _context.SaveChangesAsync();
            }
            return await _context.Milestones.Include(x => x.Major).Include(x => x.CreateByNavigation).ToListAsync();
        }

        public async Task<Milestone?> GetMilestone(int id)
        {
            return await _context.Milestones.Include(x => x.MilestoneItems).Include(x=>x.Deliverables).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Milestone>> UpdateMilestonte(Milestone milestone, int majorId)
        {
            _context.Milestones.Update(milestone);
            await _context.SaveChangesAsync();
            return await _context.Milestones
              .Include(x => x.Major)
              .Include(x => x.CreateByNavigation)
              .Include(x => x.MilestoneItems)
              .Where(x => x.IsActive == true && x.MajorId == majorId)
              .ToListAsync();
        }

        public async Task<List<Milestone>> GetByMajor(int id)
        {
            return await _context.Milestones
                .Include(x => x.Major)
                .Include(x => x.CreateByNavigation)
                .Include(x => x.MilestoneItems)
                .Where(x => x.IsActive == true && x.MajorId == id)
                .ToListAsync();
        }
    }
}
