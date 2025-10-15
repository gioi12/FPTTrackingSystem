using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Major
{
    public class MajorCategoryDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }

    }

    public class MajorDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; } 
        public List<MajorCategoryDTO>? MajorCategories { get; set; }
    }

}
