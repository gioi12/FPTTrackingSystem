using FPTTrackingSystem.Services.Staff;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/Staff/")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly IMajorService _majorService;

        public MajorController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [HttpGet("GetMajors")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _majorService.GetAllMajors();
            return StatusCode(response.Status, response);
        } 
    }
}
