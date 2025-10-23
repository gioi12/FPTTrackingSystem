using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Common.Request
{
    public class MailAnnounceRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 người nhận email")]
        public ICollection<string> To { get; set; } = new List<string>();

        [Required(ErrorMessage = "Tiêu đề email không được để trống")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Nội dung email không được để trống")]
        public string Body { get; set; }
        public List<string>? Cc { get; set; }

    }
}
