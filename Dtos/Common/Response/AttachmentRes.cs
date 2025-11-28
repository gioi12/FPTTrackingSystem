using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Common.Response
{
    public class AttachmentRes
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string UserName { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Description { get; set; }
    }
}
