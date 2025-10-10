using Entities.Models;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface ILogService
    {
        void AddLog(Log log);
        void AddRangeLog(List<Log> logList);
    }
}
