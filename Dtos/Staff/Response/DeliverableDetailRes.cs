using DataTranferObjects.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Response
{
    public class DeliverableDetailRes
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; } = null!;

        public string? Deadline { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Status { get; set; } = ProgressEnum.Unsubmitted;
        public virtual ICollection<DeliverableItemDetailRes> DeliveryItems { get; set; } = new List<DeliverableItemDetailRes>();
    }

    public class DeliverableGroupDetailDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Deadline { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class DeliverableItemDetailRes
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<attachmentItemRes> attachments { get; set; } = new List<attachmentItemRes>();

    }
    public class attachmentItemRes
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public bool isDownload { get; set; } = false;

        public string path { get; set; }
        public string userName { get; set; }
        public DateTime createAt { get; set; }
    }
}
