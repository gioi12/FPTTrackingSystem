using DataTranferObjects.Common.Evaluate;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Repositories.Common.Implements;
using Repositories.Common.Interfaces;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly AuthUtils _authUtils;

        public EvaluationService(IEvaluationRepository evaluationRepository, AuthUtils authUtils)
        {
            _evaluationRepository = evaluationRepository;
            _authUtils = authUtils;
        }

        public async Task<EvaluationResponseDTO> CreateEvaluationAsync(EvaluationCreateDTO dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Id == null)
                throw new Exception("Không thể xác thực người dùng.");

            var createdEvaluation = await _evaluationRepository.CreateEvaluationAsync(dto, user.Id.Value);

            var response = new EvaluationResponseDTO
            {
                Id = createdEvaluation.Id,
                ReceiverId = createdEvaluation.ReceiverId,
                EvaluatorId = createdEvaluation.EvaluatorId,
                Feedback = createdEvaluation.Feedback,
                GroupId = createdEvaluation.GroupId,
                DeliverableId = createdEvaluation.DeliverableId,
                CreateAt = createdEvaluation.CreateAt ?? DateTime.MinValue,
                UpdateAt = createdEvaluation.UpdateAt ?? DateTime.MinValue,
                PenaltyCards = createdEvaluation.PenatyCards?.Select(p => new PenaltyCardResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type
                }).ToList()
            };

            return response;
        }

        public async Task<List<PenaltyCardResponseDTO>> GetAllMilestonePenaltyCardsAsync()
        {
            var cards = await _evaluationRepository.GetAllPenaltyCardsAsync();

            var milestoneCards = cards
                .Where(c => c.Type != null && c.Type.Equals("Milestone", StringComparison.OrdinalIgnoreCase))
                .Select(c => new PenaltyCardResponseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Type = c.Type,
                    UserId = c.UserId ?? 0,
                    CreatedAt = c.CreateAt ?? DateTime.MinValue,
                })
                .ToList();

            return milestoneCards;
        }

        public async Task<PenaltyCardResponseDTO> CreatePenaltyCardAsync(PenaltyCardCreateDTO dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var formattedType = string.IsNullOrWhiteSpace(dto.Type)
          ? dto.Type
          : char.ToUpper(dto.Type[0]) + dto.Type.Substring(1).ToLower();

            var card = new PenatyCard
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = formattedType,
                EvaluatorId = user.Id ?? 0,
                UserId = dto.UserId == 0 ? null : dto.UserId
        };

            var created = await _evaluationRepository.CreatePenaltyCardAsync(card);

            return new PenaltyCardResponseDTO
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Type = created.Type,
                UserId = created.UserId, 
                CreatedAt = created.CreateAt ?? DateTime.UtcNow
            };
        }

        public async Task<List<PenaltyCardResponseDTO>> GetCardsByMentorIdAsync(int mentorId)
        {
            return await _evaluationRepository.GetCardsByMentorIdAsync(mentorId);
        }

        public async Task<List<EvaluationResponseDto>> GetEvaluationsByDeliverableIdAsync(int studentId)
        {
            var evaluations = await _evaluationRepository.GetByDeliverableIdAsync(studentId);

            return evaluations.Select(e => new EvaluationResponseDto
            {
                Feedback = e.Feedback,
                DeliverableName = e.Deliverable?.Name,
                CreateAt = e.CreateAt,
                EvaluatorName = e.Evaluator.Fullname,
                PenaltyCards = e.PenatyCards.Select(p => p.Name).ToList()
            }).ToList();
        }

        public async Task<List<PenaltyCardResponseDto>> GetGeneralPenaltyCardsByStudentIdAsync(int studentId)
        {
            var cards = await _evaluationRepository.GetGeneralPenaltyCardsByStudentIdAsync(studentId);

            return cards.Select(c => new PenaltyCardResponseDto
            {
                Name = c.Name,
                Description = c.Description,
                CreateAt = c.CreateAt
            }).ToList();
        }

        public async Task<List<EvaluationResponseDto>> GetEvaluationsByMentorDeliverableIdAsync(int mentorId)
        {
            var evaluations = await _evaluationRepository.GetByDeliverableMentorIdAsync(mentorId);

            return evaluations.Select(e => new EvaluationResponseDto
            {
                EvaluationId = e.Id,
                Feedback = e.Feedback,
                DeliverableName = e.Deliverable?.Name,
                CreateAt = e.CreateAt,
                EvaluatorName = e.Evaluator.Fullname,
                ReceiverId = e.ReceiverId,
                PenaltyCards = e.PenatyCards.Select(p => p.Name).ToList()
            }).ToList();
        }

        public async Task<PenatyCard?> UpdatePenaltyCardAsync(int id, PenaltyCardUpdateDTO dto)
        {
            return await _evaluationRepository.UpdatePenaltyCardAsync(id, dto.Name, dto.Description, dto.UserId);
        }

        public async Task<Evaluation?> UpdateEvaluationAsync(int id, EvaluationUpdateDTO dto)
        {
            return await _evaluationRepository.UpdateEvaluationAsync(
                id,
                dto.Feedback,
                dto.DeliverableId,
                dto.PenaltyCardIds ?? new List<int>()
            );
        }
    }
}
