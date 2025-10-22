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

        public async Task<Evaluation> CreateEvaluationAsync(Evaluation evaluation)
        {
            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();
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
