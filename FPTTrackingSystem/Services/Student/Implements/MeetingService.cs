using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using Repositories.Student.Implements;
using Repositories.Student.Interfaces;

namespace FPTTrackingSystem.Services.Student.Implements
{
    public class MeetingService : IMeetingService
    {
        private readonly IMeetingRepository _repo;
        private readonly AuthUtils _authUtils;

        public MeetingService(IMeetingRepository repo, AuthUtils authUtils)
        {
            _repo = repo;
            _authUtils = authUtils;
        }

        public async Task<object> CreateOrUpdateFreeTimeSlotsAsync(int groupId, FreeTimeSlotsRequest request)
        {
            if (request.FreeTimeSlots == null || !request.FreeTimeSlots.Any())
                throw new Exception("FreeTimeSlots cannot be empty");

            var resultList = new List<object>();

            foreach (var slot in request.FreeTimeSlots)
            {
                var existing = await _repo.GetFreeTimeSlotAsync(slot.StudentId, groupId);

                var serializedFreeTime = System.Text.Json.JsonSerializer.Serialize(slot.TimeSlots);
                var now = DateTime.UtcNow;

                if (existing != null)
                {
                    existing.FreeTime = serializedFreeTime;
                    existing.DayOfWeek = slot.DayOfWeek;
                    existing.UpdateAt = now;

                    await _repo.UpdateFreeTimeSlotAsync(existing);
                }
                else
                {
                    throw new Exception($"GroupUser not found for studentId={slot.StudentId}, groupId={groupId}");
                }

                resultList.Add(new
                {
                    dayOfWeek = slot.DayOfWeek,
                    timeSlots = slot.TimeSlots,
                    updatedAt = now
                });
            }

            await _repo.SaveChangesAsync();

            var response = new
            {
                success = true,
                message = "Free time slots updated successfully",
                data = new
                {
                    studentId = request.FreeTimeSlots.First().StudentId,
                    groupId = groupId,
                    savedSlots = resultList
                }
            };

            return response;
        }
        public async Task<List<FreeTimeSlotDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId)
        {
            return await _repo.GetFreeTimeSlotsByGroupIdAsync(groupId);
        }

        public async Task<FinalizeScheduleResponseDto> FinalizeScheduleAsync(int groupId, FinalizeScheduleRequestDto dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var meeting = await _repo.FinalizeScheduleAsync(groupId, dto.FinalMeeting, user.Id ?? 0);

            return new FinalizeScheduleResponseDto
            {
                FinalMeeting = new FinalMeetingInfo
                {
                    Id = meeting.Id,
                    IsFinalized = meeting.IsActive ?? false,
                    Day = meeting.DayOfWeek ?? string.Empty,
                    Time = meeting.Time ?? string.Empty,
                    MeetingLink = meeting.MeetingLink ?? string.Empty,
                    FinalizedAt = meeting.CreateAt ?? DateTime.UtcNow,
                    UpdatedAt = meeting.UpdateAt ?? DateTime.UtcNow
                }
            };
        }
    }
}
