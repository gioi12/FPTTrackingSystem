using DataTranferObjects.Common.Response;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IGroupService
    {
        public Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize);
        public Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int id);
        public Task<ApiResponse<List<DashBoardGroupDto>>> GetMajorGroupTotalsAsync();
        public Task<ApiResponse<GroupTrackingResponseDto>> GetGroupTrackingAsync(int groupId, DateTime startDate, DateTime endDate);
        Task<ApiResponse<List<GroupMentorDto>>> GetGroupsByUserIdAsync(int userId);
        Task<ApiResponse<string>> UpdateRoleInGroupAsync(int groupId, int userId, string newRole);
        Task<string> UploadFileGroup(IFormFile file, int groupId);
        System.Threading.Tasks.Task DeleteFileGroup(int attachmentId);
        Task<List<AttachmentRes>> GetFilesGroup(int groupId);
        Task<ApiResponse<List<GroupMentorDto>>> GetExpiredGroupsBySupervisorAsync(int supervisorId);
        Task<object> GetMockData();
        Task<object> CreateMockData();
        Task<Group> UpdateExpireDateAsync(int groupId, DateTime newExpireDate, string userRole);

    }
}
