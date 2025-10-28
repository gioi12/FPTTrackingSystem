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
        public int UserId { get; set; }
    }

    public class EvaluationCreateDTO
    {
        public int ReceiverId { get; set; }
        public int GroupId { get; set; }
        public int DeliverableId { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public List<int>? PenaltyCardIds { get; set; } 
    }

    public class PenaltyCardResponseDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class PenaltyCardResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "Milestonse";
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EvaluationResponseDTO
    {
        public int Id { get; set; }
        public int ReceiverId { get; set; }
        public int EvaluatorId { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public int? DeliverableId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<PenaltyCardResponseDTO> PenaltyCards { get; set; } = new();
    }

    public class EvaluationResponseDto
    {
        public int EvaluationId { get; set; }
        public string? Feedback { get; set; }
        public string? DeliverableName { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? EvaluatorName { get; set; }
        public int ReceiverId { get; set; }
        public List<string> PenaltyCards { get; set; } = new();
    }

    public class PenaltyCardUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? UserId { get; set; }
    }

    public class EvaluationUpdateDTO
    {
        public string? Feedback { get; set; }
        public int? DeliverableId { get; set; }
        public List<int>? PenaltyCardIds { get; set; }  
    }

}
