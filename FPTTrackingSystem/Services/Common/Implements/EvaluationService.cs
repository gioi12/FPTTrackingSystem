using DataTranferObjects.Common.Evaluate;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
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

            var evaluation = new Evaluation
            {
                ReceiverId = dto.ReceiverId,
                EvaluatorId = user.Id.Value,
                Feedback = dto.Feedback,
                GroupId = dto.GroupId,
                DeliverableId = dto.DeliverableId,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                PenatyCards = new List<PenatyCard>()
            };

            if (dto.PenaltyCards != null && dto.PenaltyCards.Any())
            {
                foreach (var card in dto.PenaltyCards)
                {
                    evaluation.PenatyCards.Add(new PenatyCard
                    {
                        Name = card.Name,
                        Description = card.Description,
                        Type = card.Type ?? "MILESTONE",
                    });
                }
            }

            var createdEvaluation = await _evaluationRepository.CreateEvaluationAsync(evaluation);

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
                PenaltyCards = createdEvaluation.PenatyCards.Select(p => new PenaltyCardResponseDTO
                {
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type,
                }).ToList()
            };

            return response;
        }

        public async Task<List<PenaltyCardResponseDTO>> GetAllMilestonePenaltyCardsAsync()
        {
            var cards = await _evaluationRepository.GetAllPenaltyCardsAsync();

            var milestoneCards = cards
                .Where(c => c.Type != null && c.Type.Equals("MILESTONE", StringComparison.OrdinalIgnoreCase))
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
            var card = new PenatyCard
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type,
                UserId = dto.UserId == 0 ? null : dto.UserId
        };

            var created = await _evaluationRepository.CreatePenaltyCardAsync(card);

            return new PenaltyCardResponseDTO
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Type = created.Type,
                UserId = created.UserId, // có thể null
                CreatedAt = created.CreateAt ?? DateTime.UtcNow
            };
        }

    }
}
