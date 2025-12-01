using DataTranferObjects.Common.Request;
using FPTTrackingSystem.Services.Admin;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FPTTrackingSystem.Controllers.Admin
{
    [Route("api/")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _service;
        private readonly IMemoryCache _cache;
        public AIController(IAIService service,IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet("v1/ai-settings")]
        public async Task<IActionResult> GetAISettings()
        {
            var settings = await _service.GetAISettings();
            return Ok(ApiResponse<object>.Success(settings, "Get AI settings successfully"));
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("v1/ai-settings")]
        public async Task<IActionResult> NewAISettings(NewAISettingsReq request)
        {
            var settings = await _service.NewAISettings(request);
            return Ok(ApiResponse<object>.Success(settings, "Get AI settings successfully"));
        }

        [HttpPost("v1/ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            var taskId = await _service.AskAsync(request.Prompt, request.GroupId);
            return Ok(ApiResponse<object>.Success(new { taskId }, "ask AI settings successfully"));

        }

        // 2️⃣ Poll kết quả theo taskId
        [HttpGet("v1/result/{taskId}")]
        public IActionResult GetResult(string taskId)
        {
            if (_cache.TryGetValue(taskId, out string result))
            {
                return Ok(ApiResponse<object>.Success(new { status = "done", result }, "ask AI settings successfully"));

            }
            return Ok(ApiResponse<object>.Success(new { status = "processing" }, "ask AI settings successfully"));
        }
    }
}
