using DataTranferObjects.Common.Evaluate;
using FPTTrackingSystem.Services.Common.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Common
{
    [Route("api/v1/Common/[controller]/")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly EvaluationService _evaluationService;

        public EvaluationController(EvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvaluation([FromBody] EvaluationCreateDTO dto)
        {
            try
            {
                var evaluation = await _evaluationService.CreateEvaluationAsync(dto);
                return Ok(new
                {
                    message = "Tạo đánh giá thành công",
                    data = evaluation
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
