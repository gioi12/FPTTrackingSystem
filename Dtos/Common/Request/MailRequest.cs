using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Common.Request
{
    public class MailRequest
    {
        public List<string> To {  get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public List<string>? Cc { get; set; }
    }
}
