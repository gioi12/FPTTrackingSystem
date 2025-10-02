using DataTranferObjects.Group;

namespace FPTTrackingSystem.Services.Group
{
    public interface IGroupService
    {
       public Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize);
        public Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int id);
    }
}
