using DataTranferObjects.Staff.Response;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff
{
    public class MilestoneRepository : IMilestoneRepository
    {
        private readonly FpttrackingSystemContext _context;

        public MilestoneRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Milestone>> NewMilestontes(List<Milestone> list)
        {
            await _context.Milestones.AddRangeAsync(list);
            await _context.SaveChangesAsync();
            /*return await _context.Milestones.Include(x => x.Semester).Include(x => x.Major).Include(x => x.CreateByNavigation).ToListAsync();*/
            return await _context.Milestones.Include(x => x.Major).Include(x => x.CreateByNavigation).ToListAsync();
        }

        public async Task<List<Milestone>> GetByMajorAndSemester(int major, int semester)
        {
            /*return await _context.Milestones.Include(x => x.Semester).Include(x => x.Major).Include(x => x.CreateByNavigation)
                .Where(x => x.SemesterId == semester && x.MajorId == major).ToListAsync();*/

            return await _context.Milestones.Include(x => x.Major).Include(x => x.CreateByNavigation)
               .Where(x => x.MajorId == major).ToListAsync();
        }

        public async Task<List<Milestone>> updateMilestontes(List<Milestone> list)
        {
            _context.Milestones.UpdateRange(list);
            await _context.SaveChangesAsync();
            /*return await _context.Milestones.Include(x => x.Semester).Include(x => x.Major).Include(x => x.CreateByNavigation).ToListAsync();*/
            return await _context.Milestones.Include(x => x.Major).Include(x => x.CreateByNavigation).ToListAsync();
        }

        public async Task<List<Milestone>> deleteMilestone(int id)
        {
            var milestone = await _context.Milestones
                         .Include(m => m.Deliverables)
                         .FirstOrDefaultAsync(x => x.Id == id);

            if (milestone != null)
            {
                _context.Deliverables.RemoveRange(milestone.Deliverables);
                _context.Milestones.Remove(milestone);
                await _context.SaveChangesAsync();
            }
            /* return await _context.Milestones.Include(x => x.Semester).Include(x => x.Major).Include(x => x.CreateByNavigation).ToListAsync();*/
            return await _context.Milestones.Include(x => x.Major).Include(x => x.CreateByNavigation).ToListAsync();
        }
    }
}
