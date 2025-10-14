
using Entities.Models;
using Repositories.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Repositories.Common.Implements
{
    public class LogRepository : ILogRepository
    {
        private readonly FpttrackingSystemContext _context;
        public LogRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task CreateLog(Log log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task CreateRangeLog(List<Log> logs)
        {
            await _context.Logs.AddRangeAsync(logs);
            await _context.SaveChangesAsync();
        }

    }
}
