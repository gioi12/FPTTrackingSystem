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
    public class EvaluationRepository : IEvaluationRepository
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
                Type = dto.Type,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            return evaluation;
        }

        public async Task<bool> CheckUserInGroupAsync(int studentId, int groupId)
        {
            return await _context.GroupUsers
                .AnyAsync(gu => gu.UserId == studentId && gu.GroupId == groupId && gu.IsActive);
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

        public async Task<Evaluation?> GetByEvaluatorReceiverDeliverableAsync(int evaluatorId, int receiverId, int deliverableId, int groupId)
        {
            return await _context.Evaluations
                .FirstOrDefaultAsync(e =>
                    e.EvaluatorId == evaluatorId &&
                    e.ReceiverId == receiverId &&
                    e.DeliverableId == deliverableId &&
                    e.GroupId == groupId
                );
        }


        public async Task<List<PenaltyCardResponseDTO>> GetCardsByMentorIdAsync(int mentorId)
        {
            return await _context.PenatyCards
                .Include(p => p.User)
                .Where(p => p.EvaluationId == mentorId && p.Type != null)
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
                .Where(e => e.ReceiverId == studentId)
                .ToListAsync();
        }
        public async Task<List<PenatyCard>> GetGeneralPenaltyCardsByStudentIdAsync(int studentId)
        {
            return await _context.PenatyCards
                .Where(p => p.UserId == studentId && p.Type != null)
                .OrderByDescending(p => p.CreateAt)
                .ToListAsync();
        }

        public async Task<List<Evaluation>> GetByDeliverableMentorIdAsync(int mentorId)
        {
            return await _context.Evaluations
               .Include(e => e.Evaluator)
               .Include(e => e.Deliverable)
               .Where(e => e.EvaluatorId == mentorId)
               .ToListAsync();
        }

        public async Task<PenatyCard?> UpdatePenaltyCardAsync(int id, string? name, string? description, int? userId, string? type)
        {
            var card = await _context.PenatyCards.FirstOrDefaultAsync(x => x.Id == id);
            if (card == null)
                return null;

            card.Name = name ?? card.Name;
            card.Description = description ?? card.Description;
            card.UserId = userId ?? card.UserId;
            card.Type = type;

            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<Evaluation?> UpdateEvaluationAsync(int id, string? feedback, int? deliverableId, string? type)
        {
            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evaluation == null)
                return null;

            evaluation.Feedback = feedback ?? evaluation.Feedback;
            evaluation.DeliverableId = deliverableId ?? evaluation.DeliverableId;
            evaluation.UpdateAt = DateTime.UtcNow;
            evaluation.Type = type;

            await _context.SaveChangesAsync();
            return evaluation;
        }

        public async Task<Evaluation?> GetEvaluationAsync(int groupId, int studentId, int deliverableId)
        {
            return await _context.Evaluations
                .Include(e => e.Evaluator)
                .Include(e => e.Deliverable)
                .Where(e =>
                    e.GroupId == groupId &&
                    e.ReceiverId == studentId &&
                    e.DeliverableId == deliverableId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Entities.Models.Task>> GetTasksByStudentAsync(int groupId, int studentId, int deliverableId)
        {
            return await _context.Tasks
                .Include(t => t.Deliverable)
                .Include(t => t.TaskUsers).ThenInclude(tu => tu.User)
                .Where(t =>
                    t.GroupId == groupId &&
                    t.DeliverableId == deliverableId &&
                    t.TaskUsers.Any(tu => tu.UserId == studentId))
                .ToListAsync();
        }

        public async Task<List<Evaluation>> GetEvaluationHistoryAsync(int groupId, int studentId, int? deliverableId)
        {
            var query = _context.Evaluations
                .Include(e => e.Deliverable)
                .Include(e => e.Receiver)
                .Include(e => e.Evaluator)
                .Where(e => e.GroupId == groupId && e.ReceiverId == studentId);

            if (deliverableId.HasValue)
                query = query.Where(e => e.DeliverableId == deliverableId.Value);

            return await query.OrderByDescending(e => e.CreateAt).ToListAsync();
        }

        public async Task<(int total, int completed)> GetTaskStatisticsAsync(int groupId, int studentId, int? deliverableId)
        {
            var query = _context.Tasks
                .Include(t => t.TaskUsers)
                .Where(t => t.GroupId == groupId &&
                            t.TaskUsers.Any(tu => tu.UserId == studentId && tu.Type == "Assignee"));

            if (deliverableId.HasValue)
                query = query.Where(t => t.DeliverableId == deliverableId.Value);

            int total = await query.CountAsync();
            int completed = await query.CountAsync(t => t.Status == "Done");

            return (total, completed);
        }


    }
}
