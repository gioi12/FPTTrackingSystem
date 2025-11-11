using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Request
{
    public class SemesterCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class SemesterUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

}
