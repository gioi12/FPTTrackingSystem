using DataTranferObjects.Student.Meeting;

namespace FPTTrackingSystem.Services.Student.Interfaces
{
    public interface IMeetingService
    {
        Task<object> CreateOrUpdateFreeTimeSlotsAsync(int groupId, FreeTimeSlotsRequest request);
        Task<List<StudentFreeTimeDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId);
        Task<FinalizeScheduleResponseDto> FinalizeScheduleAsync(int groupId, FinalizeScheduleRequestDto dto);
    }
}
