using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTranferObjects.Enum;
namespace DataTranferObjects.Common.Response
{
    public class AITaskState
    {
        public string TaskId { get; set; } = default!;
        public AIEnum Status { get; set; }
        public string? Result { get; set; }
        public string? Error { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
