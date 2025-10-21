using DataTranferObjects.Common.Evaluate;
using Entities.Models;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface IEvaluationService
    {
        Task<Evaluation> CreateEvaluationAsync(EvaluationCreateDTO dto);
    }
}
