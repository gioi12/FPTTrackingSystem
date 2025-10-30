using DataTranferObjects.Staff.Task;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Repositories.Common.Interfaces;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class CommentService: ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly AuthUtils _authUtils;

        public CommentService(ICommentRepository commentRepository, AuthUtils authUtils)
        {
            _commentRepository = commentRepository;
            _authUtils = authUtils;
        }

        public async Task<ApiResponse<CommentDTO>> CreateCommentAsync(CreateCommentDto dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                return new ApiResponse<CommentDTO>(401, "Người dùng chưa đăng nhập");

            var comment = new Comment
            {
                EntityName = "Task",
                EntityId = 1,
                Feedback = dto.Feedback,
                GroupId = dto.GroupId,
                UserId = user.Id ?? 0,
                CreateAt = DateTime.Now
            };

            var savedComment = await _commentRepository.CreateCommentAsync(comment);

            var result = new CommentDTO
            {
                Id = savedComment.Id,
                EntityName = savedComment.EntityName,
                EntityId = savedComment.EntityId,
                Feedback = savedComment.Feedback,
                GroupId = savedComment.GroupId,
                UserId = savedComment.UserId,
                CreateAt = savedComment.CreateAt
            };

            return new ApiResponse<CommentDTO>(200, "Tạo comment thành công", result);
        }
    }
}
