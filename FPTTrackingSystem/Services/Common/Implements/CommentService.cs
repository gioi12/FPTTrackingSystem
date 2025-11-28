using DataTranferObjects.Staff.Task;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Repositories.Common.Interfaces;
using Repositories.Student.Implements;
using Repositories.Student.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class CommentService: ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly AuthUtils _authUtils;
        private readonly ITaskRepository _taskRepository;

        public CommentService(ICommentRepository commentRepository, ITaskRepository taskRepository, AuthUtils authUtils)
        {
            _commentRepository = commentRepository;
            _authUtils = authUtils;
            _taskRepository = taskRepository;
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

        public async System.Threading.Tasks.Task DeleteCommentAsync(int taskId, int commentId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
                throw new ValidationException("Comment not found");

            if (comment.TaskId != taskId)
                throw new ValidationException("Comment does not belong to this task");

            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                throw new ValidationException("Task not found");

            bool isCommentOwner = comment.UserId == user.Id;
            bool isTaskOwner = task.CreatedBy == user.Id;   
            bool isSecretary = user.RoleInGroup == "Secretary";
            bool isSupervisor = user.RoleInGroup == "Supervisor";

            bool canDelete = isCommentOwner || isTaskOwner || isSecretary || isSupervisor;

            if (!canDelete)
                throw new ValidationException("You do not have permission to delete this comment");

            await _commentRepository.DeleteCommentAsync(comment);
        }

        public async Task<ApiResponse<object>> UpdateCommentAsync(int taskId, int commentId, UpdateCommentDto dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var comment = await _commentRepository.GetCommentAsync(taskId, commentId);
            if (comment == null)
                return ApiResponse<object>.Fail("Comment not found", 404);

            if (comment.UserId != user.Id)
                return ApiResponse<object>.Fail("You do not have permission to edit this comment", 403);

            if (string.IsNullOrWhiteSpace(dto.Feedback))
                return ApiResponse<object>.Fail("Feedback cannot be empty", 400);

            comment.Feedback = dto.Feedback;
            comment.CreateAt = DateTime.Now;

            await _commentRepository.UpdateCommentAsync(comment);

            return ApiResponse<object>.Success("Comment updated successfully");
        }


    }
}
