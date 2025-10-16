using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataTranferObjects.Common.Request
{
    public class UploadRequest
    {
        public int TargetId { get; set; }
        public int GroupId { get; set; }
        public int Type { get; set; }
    }
}
