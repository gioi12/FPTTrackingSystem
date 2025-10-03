using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Response
{
    public class SemesterActiveRes
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; } = null;
        public List<object>? majors {  get; set; }
    }
}
