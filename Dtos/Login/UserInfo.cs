using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Login
{
    public class UserInfo
    {
        public int? Id { get; set; }
        public int? SemesterId { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? RoleInGroup { get; set; }
        public int? CampusId { get; set; }
        public List<int>? Groups { get; set; }
    }
}
