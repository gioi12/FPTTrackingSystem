
using Entities.Models;
using Repositories.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Implements
{
    public class LogRepository : ILogRepository
    {
        private readonly FpttrackingSystemContext _context;
        public LogRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public void CreateLog(Log log)
        {
            _context.Logs.Add(log);
            _context.SaveChangesAsync();
        }

        public void CreateRangeLog(List<Log> log)
        {
            _context.Logs.AddRangeAsync(log);
            _context.SaveChangesAsync();
        }
    }
}
