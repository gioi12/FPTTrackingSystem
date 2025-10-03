using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff
{
    public interface IMilestoneService
    {
        Task<ApiResponse<List<MilestoneResponse>>> CreateMilestoneInSemester(List<MilestoneCreateRequest> request);
        Task<ApiResponse<List<MilestoneResponse>>> GetMilestoneByMajorAndSemester(int majorId , int semesterId);
        Task<ApiResponse<List<MilestoneResponse>>> UpdateInfoMilestone(List<MilestoneCreateRequest> request);
        Task<ApiResponse<List<MilestoneResponse>>> DeleteMilestone(int id);

    }
}
