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
        public int? Size { get; set; }

    }

    public class MajorDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; } 
        public List<MajorCategoryDTO>? MajorCategories { get; set; }
    }

    public class MockGroupUser
    {
        public string RollNumber { get; set; }
        public string Role { get; set; }
    }


}
