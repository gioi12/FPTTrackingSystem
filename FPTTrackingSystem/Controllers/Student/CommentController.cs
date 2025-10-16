using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Services.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Student
{
    [Route("api/v1/Student/Comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            var response = await _commentService.CreateCommentAsync(dto);
            return StatusCode(response.Status, response);
        }
    }
}
