using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Request
{
    public class MilestoneItemRequest
    {
        public int? id {  get; set; }
        [NotNull]
        public string name { get; set; }
        public string? description { get; set; }
    }
}
