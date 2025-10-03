using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff
{
    public class MajorRepository : IMajorRepository
    {
        private readonly FpttrackingSystemContext _context;
        public MajorRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<List<Major>> findAll()
        {
            return await _context.Majors.ToListAsync();
        }
    }
}
