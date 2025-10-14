using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Repositories.Common.Interfaces
{
    public interface ILogRepository
    {
        public Task CreateRangeLog(List<Log> log);

        public Task CreateLog(Log log);
    }
}
