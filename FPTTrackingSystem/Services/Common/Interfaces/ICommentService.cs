using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface ICommentService
    {
        Task<ApiResponse<CommentDTO>> CreateCommentAsync(CreateCommentDto dto);
        Task DeleteCommentAsync(int taskId, int commentId);
    }
}
