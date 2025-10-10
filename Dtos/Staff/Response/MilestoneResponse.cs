using DataTranferObjects.Staff.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Response
{
    public class MilestoneResponse
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? Description { get; set; }

        public string? Deadline { get; set; }

        public string? MajorName { get; set; }
        public string? UserCreatedName { get; set; }
        public List<MilestoneItemRequest>? Items { get; set; }
    }
}
