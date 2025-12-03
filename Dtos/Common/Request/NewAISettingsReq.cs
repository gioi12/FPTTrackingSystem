using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Common.Request
{
    public class NewAISettingsReq
    {
        public string? Name { get; set; }
        public string? SecretKey { get; set; }
    }
}
