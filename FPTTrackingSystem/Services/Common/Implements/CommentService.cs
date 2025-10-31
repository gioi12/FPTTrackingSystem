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
            try
            {
                if (dto == null)
                    return new ApiResponse<CommentDTO>(400, "Invalid request data.");

                if (dto.TaskId <= 0)
                    return new ApiResponse<CommentDTO>(400, "Invalid TaskId.");

                if (dto.GroupId <= 0)
                    return new ApiResponse<CommentDTO>(400, "Invalid GroupId.");

                if (string.IsNullOrWhiteSpace(dto.Feedback))
                    return new ApiResponse<CommentDTO>(400, "Feedback cannot be empty.");

                var user = await _authUtils.GetUserInfoFromCookie();
                if (user == null)
                    return new ApiResponse<CommentDTO>(401, "User not logged in.");

                var comment = new Comment
                {
                    TaskId = dto.TaskId,
                    Feedback = dto.Feedback.Trim(),
                    GroupId = dto.GroupId,
                    UserId = user.Id ?? 0,
                    CreateAt = DateTime.Now
                };

                var savedComment = await _commentRepository.CreateCommentAsync(comment);

                var result = new CommentDTO
                {
                    Id = savedComment.Id,
                    Feedback = savedComment.Feedback,
                    GroupId = savedComment.GroupId,
                    UserId = savedComment.UserId,
                    CreateAt = savedComment.CreateAt
                };

                return new ApiResponse<CommentDTO>(200, "Comment created successfully.", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateCommentAsync] Error: {ex.Message}");
                return new ApiResponse<CommentDTO>(500, "An error occurred while creating the comment.");
            }
        }

    }
}
