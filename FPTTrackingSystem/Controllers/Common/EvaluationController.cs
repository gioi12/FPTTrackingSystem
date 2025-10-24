using DataTranferObjects.Common.Evaluate;
using FPTTrackingSystem.Services.Common.Implements;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Common
{
    [Route("api/v1/Common/[controller]/")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        private readonly AuthUtils _authUtils;

        public EvaluationController(IEvaluationService evaluationService, AuthUtils authUtils)
        {
            _evaluationService = evaluationService;
            _authUtils = authUtils;
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

        [HttpGet("getCardGeneralFromMentorId")]
        public async Task<IActionResult> GetCardsByMentorId()
        {
            var mentor = await _authUtils.GetUserInfoFromCookie();
            var cards = await _evaluationService.GetCardsByMentorIdAsync(mentor.Id ?? 0);

            if (cards == null || !cards.Any())
                return NotFound(new { message = "Không tìm thấy thẻ phạt nào cho mentor này." });

            return Ok(cards);
        }

        [HttpGet("getEvaluationFromDeliverableByStudent")]
        public async Task<IActionResult> GetByDeliverable()
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var result = await _evaluationService.GetEvaluationsByDeliverableIdAsync(user.Id ?? 0);

            if (result == null || !result.Any())
                return NotFound(new { Message = "Không có đánh giá nào cho deliverable này." });

            return Ok(result);
        }

        [HttpGet("getEvaluationByMentorDeliverable")]
        public async Task<IActionResult> GetByDeliverableMentor()
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var result = await _evaluationService.GetEvaluationsByMentorDeliverableIdAsync(user.Id ?? 0);

            if (result == null || !result.Any())
                return NotFound(new { Message = "Không có đánh giá nào cho deliverable này." });

            return Ok(result);
        }

        [HttpGet("getCardEvaluationGeneralByStudent")]
        public async Task<IActionResult> GetGeneralByStudent()
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var result = await _evaluationService.GetGeneralPenaltyCardsByStudentIdAsync(user.Id ?? 0);

            if (result == null || !result.Any())
                return NotFound(new { Message = "Không có thẻ phạt General nào cho sinh viên này." });

            return Ok(result);
        }

        [HttpPut("update/penalty-card/{id}")]
        public async Task<IActionResult> UpdatePenaltyCard(int id, [FromBody] PenaltyCardUpdateDTO dto)
        {
            var result = await _evaluationService.UpdatePenaltyCardAsync(id, dto);
            if (result == null)
                return NotFound(ApiResponse<string>.Fail("Penalty card not found", 404));

            return Ok(ApiResponse<object>.Success(new
            {
                result.Id,
                result.Name,
                result.Description,
                result.UserId,
                result.CreateAt
            }, "Update penalty card success"));
        }

        [HttpPut("update/evaluation/{id}")]
        public async Task<IActionResult> UpdateEvaluation(int id, [FromBody] EvaluationUpdateDTO dto)
        {
            var result = await _evaluationService.UpdateEvaluationAsync(id, dto);
            if (result == null)
                return NotFound(ApiResponse<string>.Fail("Evaluation not found", 404));

            return Ok(ApiResponse<object>.Success(new
            {
                result.Id,
                result.Feedback,
                result.DeliverableId,
                result.UpdateAt,
                PenaltyCards = result.PenatyCards.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Type
                }).ToList()
            }, "Cập nhật Evaluation thành công."));
        }
    }
}
