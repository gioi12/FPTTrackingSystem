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
    }
}
