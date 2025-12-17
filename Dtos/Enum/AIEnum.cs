using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Enum
{
    public enum AIEnum
    {
        Pending,    // mới tạo
        Processing, // consumer đang xử lý
        Done,       // thành công
        Failed,     // lỗi
        Timeout     // quá thời gian
    }
}
