using DataTranferObjects.Group;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff
{
    public interface IGroupService
    {
        public Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize);
        public Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int id);
        public Task<ApiResponse<List<DashBoardGroupDto>>> GetMajorGroupTotalsAsync();
    }
}
