using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IMilestoneService
    {
        Task<ApiResponse<List<MilestoneResponse>>> CreateMilestoneInSemester(List<MilestoneCreateRequest> request);
        Task<ApiResponse<List<MilestoneResponse>>> GetMilestonesByMajor(int majorCateId);
        Task<ApiResponse<List<MilestoneResponse>>> UpdateInfoMilestone(MilestoneUpdateRequest request);
        Task<ApiResponse<List<MilestoneResponse>>> DeleteMilestone(int id);
        Task<List<MilestonesDTO>> GetMilestonesByGroupIdAsync(int groupId, int semesterId);
    }
}
