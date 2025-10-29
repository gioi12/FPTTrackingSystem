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
                throw new UnauthorizedAccessException("User authentication failed.");

            if (user.Role != "Supervisor")
                throw new UnauthorizedAccessException("Only mentors are allowed create evaluation.");

            if (dto.ReceiverId <= 0)
                throw new ArgumentException("ReceiverId must be a positive number.");
            if (dto.GroupId <= 0)
                throw new ArgumentException("GroupId must be a positive number.");
            if (dto.DeliverableId <= 0)
                throw new ArgumentException("DeliverableId must be a positive number.");

            if (!string.IsNullOrWhiteSpace(dto.Feedback) && dto.Feedback.Length > 500)
                throw new ArgumentException("Feedback cannot exceed 500 characters.");

            if (string.IsNullOrWhiteSpace(dto.Type))
                throw new ArgumentException("Type is required.");

            var allowedTypes = new[] { "excellent", "good", "fair", "average", "poor" };
            var typeLower = dto.Type.Trim().ToLower();

            if (!allowedTypes.Contains(typeLower))
                throw new ArgumentException("Type must be one of: Excellent, Good, Fair, Average, Poor.");

            var formattedType = char.ToUpper(typeLower[0]) + typeLower.Substring(1);

            var isMentorInGroup = await _evaluationRepository.CheckUserInGroupAsync(user.Id.Value, dto.GroupId);
            if (!isMentorInGroup)
                throw new UnauthorizedAccessException("You are not the mentor of this group and cannot evaluate its members.");

            var isStudentInGroup = await _evaluationRepository.CheckUserInGroupAsync(dto.ReceiverId, dto.GroupId);
            if (!isStudentInGroup)
                throw new UnauthorizedAccessException("The student is not part of this group.");

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
                Type = formattedType

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
            if (user == null || user.Id == null)
                throw new UnauthorizedAccessException("User authentication failed.");

            if (user.Role != "Supervisor")
                throw new UnauthorizedAccessException("Only mentors are allowed create evaluation.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");
            if (dto.Name.Length > 100)
                throw new ArgumentException("Name cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.");

            if (string.IsNullOrWhiteSpace(dto.Type))
                throw new ArgumentException("Type is required.");

            if (dto.UserId < 0)
                throw new ArgumentException("UserId must be greater than 0 or null.");

            var allowedTypes = new[] { "warning", "no-deduction", "deduction" };
            var typeLower = dto.Type.Trim().ToLower();

            if (!allowedTypes.Contains(typeLower))
                throw new ArgumentException("Type must be one of: warning, no-deduction, deduction.");

            var formattedType = char.ToUpper(typeLower[0]) + typeLower.Substring(1);

            var card = new PenatyCard
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = formattedType,
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
                Type = e.Type
            }).ToList();
        }

        public async Task<List<PenaltyCardResponseDto>> GetGeneralPenaltyCardsByStudentIdAsync(int studentId)
        {
            var cards = await _evaluationRepository.GetGeneralPenaltyCardsByStudentIdAsync(studentId);

            return cards.Select(c => new PenaltyCardResponseDto
            {
                Type = c.Type,
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
                Type = e.Type,
                EvaluationId = e.Id,
                Feedback = e.Feedback,
                DeliverableName = e.Deliverable?.Name,
                CreateAt = e.CreateAt,
                EvaluatorName = e.Evaluator.Fullname,
                ReceiverId = e.ReceiverId,
            }).ToList();
        }

        public async Task<PenatyCard?> UpdatePenaltyCardAsync(int id, PenaltyCardUpdateDTO dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Id == null)
                throw new UnauthorizedAccessException("User authentication failed.");

            if (user.Role != "Supervisor")
                throw new UnauthorizedAccessException("Only mentors are allowed create evaluation.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");
            if (dto.Name.Length > 100)
                throw new ArgumentException("Name cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.");

            if (string.IsNullOrWhiteSpace(dto.Type))
                throw new ArgumentException("Type is required.");

            if (dto.UserId < 0)
                throw new ArgumentException("UserId must be greater than 0 or null.");

            var allowedTypes = new[] { "warning", "no-deduction", "deduction" };
            var typeLower = dto.Type.Trim().ToLower();

            if (!allowedTypes.Contains(typeLower))
                throw new ArgumentException("Type must be one of: warning, no-deduction, deduction.");

            var formattedType = char.ToUpper(typeLower[0]) + typeLower.Substring(1);
            return await _evaluationRepository.UpdatePenaltyCardAsync(id, dto.Name, dto.Description, dto.UserId,dto.Type);
        }

        public async Task<Evaluation?> UpdateEvaluationAsync(int id, EvaluationUpdateDTO dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Id == null)
                throw new UnauthorizedAccessException("User authentication failed.");

            if (user.Role != "Supervisor")
                throw new UnauthorizedAccessException("Only mentors are allowed create evaluation.");

            if (!string.IsNullOrWhiteSpace(dto.Feedback) && dto.Feedback.Length > 500)
                throw new ArgumentException("Feedback cannot exceed 500 characters.");

            if (dto.DeliverableId.HasValue && dto.DeliverableId.Value <= 0)
                throw new ArgumentException("DeliverableId must be a positive number if provided.");

            string? formattedType = null;
            if (!string.IsNullOrWhiteSpace(dto.Type))
            {
                var allowedTypes = new[] { "excellent", "good", "fair", "average", "poor" };
                var typeLower = dto.Type.Trim().ToLower();

                if (!allowedTypes.Contains(typeLower))
                    throw new ArgumentException("Type must be one of: Excellent, Good, Fair, Average, Poor.");

                formattedType = char.ToUpper(typeLower[0]) + typeLower.Substring(1);
            }

            return await _evaluationRepository.UpdateEvaluationAsync(
                id,
                dto.Feedback,
                dto.DeliverableId,
                formattedType
            );
        }
    }
}
