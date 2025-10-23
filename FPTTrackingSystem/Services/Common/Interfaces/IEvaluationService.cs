using DataTranferObjects.Common.Evaluate;
using Entities.Models;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface IEvaluationService
    {
        Task<EvaluationResponseDTO> CreateEvaluationAsync(EvaluationCreateDTO dto);
        Task<List<PenaltyCardResponseDTO>> GetAllMilestonePenaltyCardsAsync();
        Task<PenaltyCardResponseDTO> CreatePenaltyCardAsync(PenaltyCardCreateDTO dto);
        Task<List<PenaltyCardResponseDTO>> GetCardsByMentorIdAsync(int mentorId);
    }
}
