using DataTranferObjects.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Response
{
    public class GroupDeliverableRes
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; } = null!;

        public string? Deadline { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Status { get; set; } = ProgressEnum.Unsubmitted;
        public virtual ICollection<DeliverableItemRes> DeliveryItems { get; set; } = new List<DeliverableItemRes>();
    }
}
