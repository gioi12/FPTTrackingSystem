using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Task
{
    public class taskDTO
    {
    }
    public class CreateTaskDTO
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EndAt { get; set; }

        public int AssignedUserId { get; set; }
    }

}
