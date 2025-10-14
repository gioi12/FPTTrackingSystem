using Entities.Models;
using Task = System.Threading.Tasks.Task;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface ILogService
    {
       Task AddLogAsync(Log log);
       Task AddRangeLogAsync(List<Log> logList);
    }
}
