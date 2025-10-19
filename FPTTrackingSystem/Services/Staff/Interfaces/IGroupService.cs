using DataTranferObjects.Staff.Group;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IGroupService
    {
        public Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize);
        public Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int id);
        public Task<ApiResponse<List<DashBoardGroupDto>>> GetMajorGroupTotalsAsync();
        public Task<ApiResponse<GroupTrackingResponseDto>> GetGroupTrackingAsync(int groupId, DateTime startDate, DateTime endDate);
        Task<List<GroupMentorDto>> GetGroupsByUserIdAsync(int userId);
        Task<ApiResponse<string>> UpdateRoleInGroupAsync(int groupId, int userId, string newRole);
    }
}
