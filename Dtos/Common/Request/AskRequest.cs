using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Common.Request
{
    public class AskRequest
    {
        public string Prompt { get; set; }
        public int? GroupId { get; set; }
    }
}
