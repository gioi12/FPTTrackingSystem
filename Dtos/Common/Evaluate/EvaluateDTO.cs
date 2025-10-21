using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Common.Evaluate
{
    public class EvaluateDTO
    {
    }

    public class PenaltyCardCreateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int? UserId { get; set; }
    }

    public class EvaluationCreateDTO
    {
        public int ReceiverId { get; set; }
        public string? Feedback { get; set; }
        public string? Type { get; set; }
        public int GroupId { get; set; }
        public int? DeliverableId { get; set; }

        public List<PenaltyCardCreateDTO>? PenaltyCards { get; set; }
    }

}
