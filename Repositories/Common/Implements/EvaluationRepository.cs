using DataTranferObjects.Common.Evaluate;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Common.Interfaces;
using Repositories.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Implements
{
    public class EvaluationRepository: IEvaluationRepository
    {
        private readonly FpttrackingSystemContext _context;

        public EvaluationRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<Evaluation> CreateEvaluationAsync(EvaluationCreateDTO dto, int evaluatorId)
        {
            var evaluation = new Evaluation
            {
                ReceiverId = dto.ReceiverId,
                EvaluatorId = evaluatorId,
                Feedback = dto.Feedback,
                GroupId = dto.GroupId,
                DeliverableId = dto.DeliverableId,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            if (dto.PenaltyCardIds != null && dto.PenaltyCardIds.Any())
            {
                var cards = await _context.PenatyCards
                    .Where(p => dto.PenaltyCardIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var card in cards)
                {
                    card.EvaluationId = evaluation.Id;
                }

                await _context.SaveChangesAsync();
            }

            evaluation.PenatyCards = await _context.PenatyCards
                .Where(p => p.EvaluationId == evaluation.Id)
                .ToListAsync();

            return evaluation;
        }

        public async Task<List<PenatyCard>> GetAllPenaltyCardsAsync()
        {
            return await _context.PenatyCards.ToListAsync();
        }

        public async Task<PenatyCard> CreatePenaltyCardAsync(PenatyCard card)
        {
            card.CreateAt = DateTime.UtcNow;
            _context.PenatyCards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<List<PenaltyCardResponseDTO>> GetCardsByMentorIdAsync(int mentorId)
        {
            return await _context.PenatyCards
                .Include(p => p.User)
                .Where(p => p.EvaluatorId == mentorId && p.Type != null && p.Type.ToLower() == "general")
                .Select(p => new PenaltyCardResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type,
                    UserId = p.UserId,
                    UserName = p.User != null ? p.User.Fullname : null
                })
                .ToListAsync();
        }

        public async Task<List<Evaluation>> GetByDeliverableIdAsync(int studentId)
        {
            return await _context.Evaluations
                .Include(e => e.Evaluator)
                .Include(e => e.Deliverable)
                .Include(e => e.PenatyCards)
                .Where(e => e.ReceiverId == studentId)
                .ToListAsync();
        }
        public async Task<List<PenatyCard>> GetGeneralPenaltyCardsByStudentIdAsync(int studentId)
        {
            return await _context.PenatyCards
                .Where(p => p.UserId == studentId && p.Type != null && p.Type.ToLower() == "general")
                .OrderByDescending(p => p.CreateAt)
                .ToListAsync();
        }

        public async Task<List<Evaluation>> GetByDeliverableMentorIdAsync(int mentorId)
        {
            return await _context.Evaluations
               .Include(e => e.Evaluator)
               .Include(e => e.Deliverable)
               .Include(e => e.PenatyCards)
               .Where(e => e.EvaluatorId == mentorId)
               .ToListAsync();
        }

        public async Task<PenatyCard?> UpdatePenaltyCardAsync(int id, string? name, string? description, int? userId)
        {
            var card = await _context.PenatyCards.FirstOrDefaultAsync(x => x.Id == id);
            if (card == null)
                return null;

            card.Name = name ?? card.Name;
            card.Description = description ?? card.Description;
            card.UserId = userId ?? card.UserId;

            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<Evaluation?> UpdateEvaluationAsync(int id, string? feedback, int? deliverableId, List<int>? penaltyCardIds)
        {
            var evaluation = await _context.Evaluations
                .Include(e => e.PenatyCards)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evaluation == null)
                return null;

            evaluation.Feedback = feedback ?? evaluation.Feedback;
            evaluation.DeliverableId = deliverableId ?? evaluation.DeliverableId;
            evaluation.UpdateAt = DateTime.UtcNow;

            if (penaltyCardIds != null)
            {
                foreach (var card in evaluation.PenatyCards.ToList())
                {
                    card.EvaluationId = null;
                }

                var newCards = await _context.PenatyCards
                    .Where(c => penaltyCardIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var card in newCards)
                {
                    card.EvaluationId = evaluation.Id;
                }
            }

            await _context.SaveChangesAsync();
            return evaluation;
        }
    }
}
