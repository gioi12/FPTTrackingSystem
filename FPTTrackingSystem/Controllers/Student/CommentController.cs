using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Services.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Student
{
    [Route("api/v1/")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("Student/Comment/create")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            var response = await _commentService.CreateCommentAsync(dto);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("/Student/Comment/task/{taskId}/comment/{commentId}")]
        public async Task<IActionResult> DeleteComment(int taskId, int commentId)
        {
            await _commentService.DeleteCommentAsync(taskId, commentId);

            return Ok(new
            {
                Status = 200,
                Message = "Deleted comment successfully"
            });
        }

        [HttpPut("/Student/Comment/task/{taskId}/comment/{commentId}")]
        public async Task<IActionResult> UpdateComment(int taskId, int commentId, [FromBody] UpdateCommentDto dto)
        {
            var response = await _commentService.UpdateCommentAsync(taskId, commentId, dto);
            return StatusCode(response.Status, response);
        }


    }
}
