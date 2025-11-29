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
        Task<List<EvaluationResponseDto>> GetEvaluationsByDeliverableIdAsync(int studentId);
        Task<List<EvaluationResponseDto>> GetEvaluationsByMentorDeliverableIdAsync(int mentorId);
        Task<List<PenaltyCardResponseDto>> GetGeneralPenaltyCardsByStudentIdAsync(int studentId);
        Task<PenatyCard?> UpdatePenaltyCardAsync(int id, PenaltyCardUpdateDTO dto);
        Task<Evaluation?> UpdateEvaluationAsync(int id, EvaluationUpdateDTO dto);
        Task<object> GetStudentEvaluationDetail(int groupId, int studentId, int deliverableId);
        Task<object> GetStudentStatisticsAsync(int groupId, int studentId, int? deliverableId);
    }
}
