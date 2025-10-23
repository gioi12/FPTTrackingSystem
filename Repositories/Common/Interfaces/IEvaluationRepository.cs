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
        Task<Evaluation> CreateEvaluationAsync(Evaluation evaluation);
        Task<List<PenatyCard>> GetAllPenaltyCardsAsync();
        Task<PenatyCard> CreatePenaltyCardAsync(PenatyCard card);
    }
}
