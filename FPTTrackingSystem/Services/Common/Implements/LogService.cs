using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using Repositories.Common.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task AddLogAsync(Log log)
        {
            await _logRepository.CreateLog(log);
        }

        public async Task AddRangeLogAsync(List<Log> logList)
        {
            await _logRepository.CreateRangeLog(logList);
        }
    }
}
