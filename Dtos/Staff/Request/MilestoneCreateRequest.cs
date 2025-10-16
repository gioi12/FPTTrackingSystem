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
        [NotNull]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [NotNull]
        public int MajorCateId { get; set; }
       
    }
}
