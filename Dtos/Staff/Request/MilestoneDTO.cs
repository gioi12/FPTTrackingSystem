using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Request
{
    public class MilestonesDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Deadline { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? CreateBy { get; set; }
        public int MajorId { get; set; }
    }
}
