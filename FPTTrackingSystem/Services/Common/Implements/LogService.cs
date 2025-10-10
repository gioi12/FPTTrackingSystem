using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using Repositories.Common.Interfaces;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public void AddLog(Log log)
        {
            _logRepository.CreateLog(log);
        }

        public void AddRangeLog(List<Log> logList)
        {
            _logRepository.CreateRangeLog(logList);
        }
    }
}
