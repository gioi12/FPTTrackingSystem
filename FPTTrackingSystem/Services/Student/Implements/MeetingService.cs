using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using Mapster;
using Repositories.Student.Implements;
using Repositories.Student.Interfaces;
using System.ComponentModel.DataAnnotations;

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

        public async Task<MeetingMinuteRes> CreateMeetingMinute(MeetingMinuteRequest request)
        {
            var meetingMinute = await _repo.GetMeetingMinuteByMeeting(request.MeetingId);
            if(meetingMinute != null)
            {
                throw new ValidationException("Meeting minute already exists for this meeting.");
            }
            var user = await _authUtils.GetUserInfoFromCookie();

            MeetingMinute newMinute = new MeetingMinute
            {
                MeetingId = request.MeetingId,
                MeetingMinusDate = request.MeetingMinusDate,
                Attendance = request.Attendance,
                Issue = request.Issue,
                MeetingContent = request.MeetingContent,
                Other = request.Other,
                CreateAt = DateTime.Now,
                CreateBy = user.Id
            };
            var met = await _repo.CreateMeetingMinute(newMinute);
            return met.Adapt<MeetingMinuteRes>();
        }

        public async Task<MeetingMinuteRes> GetMeetingMinute(int meetingId)
        {
            var meetingMinute = await _repo.GetMeetingMinuteByMeeting(meetingId);
            return meetingMinute.Adapt<MeetingMinuteRes>();
        }

        public async Task<MeetingMinuteRes> UpdateMeetingMinute(MeetingMinuteUpdateReq req)
        {
            var meetingMinute = await _repo.GetMeetingMinuteById(req.Id);
            if (meetingMinute == null)
            {
                throw new ValidationException("Meeting not found.");
            }
            meetingMinute.Issue = req.Issue;
            meetingMinute.MeetingMinusDate = req.MeetingMinusDate;
            meetingMinute.Attendance = req.Attendance;
            meetingMinute.MeetingContent = req.MeetingContent;
            meetingMinute.Other = req.Other;

            var res =  await _repo.UpdateMeetingMinute(meetingMinute);
            return res.Adapt<MeetingMinuteRes>();
        }

        public async System.Threading.Tasks.Task DeleteMeetingMinute(int id)
        {
            var meetMinu = await _repo.GetMeetingMinuteById(id);
            if(meetMinu == null)
            {
                throw new ValidationException("Meeting minute not found.");
            }
            await _repo.DeleteMeetingMinute(meetMinu);
        }
    }
}
