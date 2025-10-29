using DataTranferObjects.Common.Evaluate;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Interfaces
{
    public interface IEvaluationRepository
    {
        Task<Evaluation> CreateEvaluationAsync(EvaluationCreateDTO dto, int evaluatorId);
        Task<List<PenatyCard>> GetAllPenaltyCardsAsync();
        Task<PenatyCard> CreatePenaltyCardAsync(PenatyCard card);
        Task<List<PenaltyCardResponseDTO>> GetCardsByMentorIdAsync(int mentorId);
        Task<List<Evaluation>> GetByDeliverableMentorIdAsync(int mentorId);
        Task<List<Evaluation>> GetByDeliverableIdAsync(int studentId);
        Task<List<PenatyCard>> GetGeneralPenaltyCardsByStudentIdAsync(int studentId);
        Task<PenatyCard?> UpdatePenaltyCardAsync(int id, string? name, string? description, int? userId);
        Task<Evaluation?> UpdateEvaluationAsync(int id, string? feedback, int? deliverableId, string? type);
        Task<bool> CheckUserInGroupAsync(int studentId, int groupId);
    }
}
