using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Request
{
    public class MilestoneCreateRequest
    {
        public int? Id {  get; set; }
        [NotNull]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Deadline { get; set; }
        [NotNull]
        public int MajorId { get; set; }
        [NotNull]
        public int SemesterId { get; set; }
    }
}
