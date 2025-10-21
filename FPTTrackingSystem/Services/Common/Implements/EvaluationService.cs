using DataTranferObjects.Common.Evaluate;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using Repositories.Common.Implements;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class EvaluationService : IEvaluationService
    {
        private readonly EvaluationRepository _evaluationRepository;
        private readonly AuthUtils _authUtils;

        public EvaluationService(EvaluationRepository evaluationRepository, AuthUtils authUtils)
        {
            _evaluationRepository = evaluationRepository;
            _authUtils = authUtils;
        }

        public async Task<Evaluation> CreateEvaluationAsync(EvaluationCreateDTO dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Id == null)
                throw new Exception("Không thể xác thực người dùng.");

            var evaluation = new Evaluation
            {
                ReceiverId = dto.ReceiverId,
                EvaluatorId = user.Id.Value,
                Feedback = dto.Feedback,
                Type = dto.Type,
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
                        Type = card.Type,
                        UserId = card.UserId
                    });
                }
            }

            return await _evaluationRepository.CreateEvaluationAsync(evaluation);
        }
    }
}
