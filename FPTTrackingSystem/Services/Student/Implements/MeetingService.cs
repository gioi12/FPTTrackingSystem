using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using Repositories.Student.Interfaces;

namespace FPTTrackingSystem.Services.Student.Implements
{
    public class MeetingService : IMeetingService
    {
        private readonly IMeetingRepository _repo;

        public MeetingService(IMeetingRepository repo)
        {
            _repo = repo;
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


    }
}
