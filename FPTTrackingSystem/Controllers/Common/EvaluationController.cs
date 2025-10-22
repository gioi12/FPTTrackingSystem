using DataTranferObjects.Common.Evaluate;
using FPTTrackingSystem.Services.Common.Implements;
using FPTTrackingSystem.Services.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Common
{
    [Route("api/v1/Common/[controller]/")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;

        public EvaluationController(IEvaluationService evaluationService)
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

        [HttpGet("card-milestonse")]
        public async Task<IActionResult> GetMilestonePenaltyCards()
        {
            var result = await _evaluationService.GetAllMilestonePenaltyCardsAsync();
            return Ok(new
            {
                status = 200,
                message = "Lấy danh sách PenaltyCard Milestone thành công",
                data = result
            });
        }

        [HttpPost("create-card")]
        public async Task<IActionResult> CreatePenaltyCard([FromBody] PenaltyCardCreateDTO dto)
        {
            try
            {
                var result = await _evaluationService.CreatePenaltyCardAsync(dto);
                return Ok(new
                {
                    status = 200,
                    message = "Tạo thẻ phạt thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = ex.Message,
                    data = (object?)null
                });
            }
        }
    }
}
