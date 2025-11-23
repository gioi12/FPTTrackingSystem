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
                return Ok(ApiResponse<object>.Success(evaluation, "Tạo đánh giá thành công", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<string>.Forbidden(ex.Message));
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
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
                return Ok(ApiResponse<object>.Success(result, "Tạo thẻ phạt thành công", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<string>.Forbidden(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError(ex.Message));
            }
        }

        [HttpGet("getCardGeneralFromMentorId")]
        public async Task<IActionResult> GetCardsByMentorId()
        {
            var mentor = await _authUtils.GetUserInfoFromCookie();
            var cards = await _evaluationService.GetCardsByMentorIdAsync(mentor.Id ?? 0);

            if (cards == null || !cards.Any())
                return Ok(new { Status = 200, message = "Không tìm thấy thẻ phạt nào cho mentor này.", Data = new List<EvaluationResponseDto>() });

            return Ok(cards);
        }

        [HttpGet("getEvaluationFromDeliverableByStudent")]
        public async Task<IActionResult> GetByDeliverable()
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var result = await _evaluationService.GetEvaluationsByDeliverableIdAsync(user.Id ?? 0);

            if (result == null || !result.Any())
                return Ok(new
                {
                    Status = 200,
                    Message = "Không có đánh giá nào cho deliverable này.",
                    Data = new List<EvaluationResponseDto>()
                });


            return Ok(result);
        }

        [HttpGet("getCardEvaluationGeneralByStudent")]
        public async Task<IActionResult> GetGeneralByStudent()
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var result = await _evaluationService.GetGeneralPenaltyCardsByStudentIdAsync(user.Id ?? 0);

            if (result == null || !result.Any())
                return Ok(new { Status = 200, Message = "Không có thẻ phạt General nào cho sinh viên này.", Data = new List<EvaluationResponseDto>() });

            return Ok(result);
        }

        [HttpPut("update/penalty-card/{id}")]
        public async Task<IActionResult> UpdatePenaltyCard(int id, [FromBody] PenaltyCardUpdateDTO dto)
        {
            try
            {
                var result = await _evaluationService.UpdatePenaltyCardAsync(id, dto);
                if (result == null)
                    return Ok(new { Status = 200, Message =  "Penalty card not found" ,Data = new List<EvaluationResponseDto>() });

                return Ok(ApiResponse<object>.Success(new
                {
                    result.Id,
                    result.Name,
                    result.Description,
                    result.UserId,
                    result.Type,
                    result.CreateAt
                }, "Cập nhật penalty card thành công", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<string>.Forbidden(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError(ex.Message));
            }
        }

        [HttpPut("update/evaluation/{id}")]
        public async Task<IActionResult> UpdateEvaluation(int id, [FromBody] EvaluationUpdateDTO dto)
        {
            try
            {
                var result = await _evaluationService.UpdateEvaluationAsync(id, dto);
                if (result == null)
                    return Ok(new { Status = 200, mesage ="Evaluation not found", Data = new List<EvaluationResponseDto>() });

                return Ok(ApiResponse<object>.Success(new
                {
                    result.Id,
                    result.Feedback,
                    result.DeliverableId,
                    result.UpdateAt,
                    result.Type,
                }, "Cập nhật Evaluation thành công."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<string>.Forbidden(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError(ex.Message));
            }
        }
    }
}
