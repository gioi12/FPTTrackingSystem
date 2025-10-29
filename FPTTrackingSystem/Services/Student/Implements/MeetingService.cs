using DataTranferObjects.Enum;
using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Mapster;
using Repositories.Student.Implements;
using Repositories.Student.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role != "Student")
                throw new UnauthorizedAccessException("Only students are allowed to update free time slots.");

            var isInGroup = await _repo.CheckStudentInGroupAsync(user.Id ?? 0, groupId);
            if (!isInGroup)
                throw new UnauthorizedAccessException("You are not a member of this group.");
            if (request.FreeTimeSlots == null || !request.FreeTimeSlots.Any())
                throw new Exception("FreeTimeSlots cannot be empty");

            var resultList = new List<object>();
            var now = DateTime.UtcNow;

            foreach (var slot in request.FreeTimeSlots)
            {
                var existing = await _repo.GetFreeTimeSlotAsync(slot.StudentId, groupId);

                if (existing == null)
                {
                    throw new Exception($"GroupUser not found for studentId={slot.StudentId}, groupId={groupId}");
                }

                // Gộp toàn bộ times cho từng ngày
                var days = slot.TimeSlots.Select(d => CapitalizeFirstLetter(d.DayOfWeek));
                existing.DayOfWeek = string.Join(", ", days);

                // Tạo mảng 2 chiều cho time slots
                var timesArray = slot.TimeSlots.Select(d => d.TimeSlots).ToList();
                existing.FreeTime = System.Text.Json.JsonSerializer.Serialize(timesArray);
                existing.UpdateAt = now;


                await _repo.UpdateFreeTimeSlotAsync(existing);

                resultList.Add(new
                {
                    studentId = slot.StudentId,
                    groupId = groupId,
                    dayOfWeek = existing.DayOfWeek,
                    freeTime = existing.FreeTime,
                    updatedAt = now
                });
            }

            await _repo.SaveChangesAsync();

            return new
            {
                success = true,
                message = "Free time slots updated successfully",
                data = resultList
            };
        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public async Task<List<StudentFreeTimeDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role == "Student")
            {
                var isInGroup = await _repo.CheckStudentInGroupAsync(user.Id ?? 0, groupId);
                if (!isInGroup)
                    throw new UnauthorizedAccessException("You are not a member of this group.");
            }
            return await _repo.GetFreeTimeSlotsByGroupIdAsync(groupId);
        }

        public async Task<FinalizeScheduleResponseDto> FinalizeScheduleAsync(int groupId, FinalizeScheduleRequestDto dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role != "Mentor")
                throw new UnauthorizedAccessException("Only mentors are allowed to finalize meeting schedules.");

            var isMentorOfGroup = await _repo.CheckStudentInGroupAsync(user.Id ?? 0, groupId);
            if (!isMentorOfGroup)
                throw new UnauthorizedAccessException("You are not the mentor of this group.");
            var meeting = await _repo.FinalizeOrUpdateScheduleAsync(groupId, dto.FinalMeeting, user.Id ?? 0);

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
            var meeting = await _repo.GetMeetingDateByIdAsync(request.MeetingDateId);
            if(meeting == null)
            {
                throw new ValidationException("Meeting not found.");
            }
            if (meeting.MeetingMinute != null)
            {
                throw new ValidationException("Meeting minute already exists for this meeting.");
            }
            var user = await _authUtils.GetUserInfoFromCookie();
            var roleUser = await _repo.CheckSecretary((int)user.Id);
            if (!roleUser)
            {
                throw new ValidationException("User not Secretary");
            }
            MeetingMinute newMinute = new MeetingMinute
            {
                MeetingScheduleDateId = request.MeetingDateId,
                StartAt = request.startAt,
                EndAt = request.endAt,
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

        public async Task<MeetingMinuteRes> GetMeetingMinuteDate(int meetingDateId)
        {
            var meeting = await _repo.GetMeetingDateByIdAsync(meetingDateId);
            if (meeting == null)
            {
                throw new ValidationException("Meeting not found.");
            }
            var meetingMinute = await _repo.GetMeetingMinuteByMeetingDate(meetingDateId);
            return meetingMinute.Adapt<MeetingMinuteRes>();
        }

        public async Task<MeetingMinuteRes> UpdateMeetingMinute(MeetingMinuteUpdateReq req)
        {
            var meetingMinute = await _repo.GetMeetingMinuteById(req.Id);
            if (meetingMinute == null)
            {
                throw new ValidationException("Meeting minute not found.");
            }
            meetingMinute.Issue = req.Issue;
            meetingMinute.StartAt = req.startAt;
            meetingMinute.EndAt = req.endAt;
            meetingMinute.Attendance = req.Attendance;
            meetingMinute.MeetingContent = req.MeetingContent;
            meetingMinute.Other = req.Other;

            var res = await _repo.UpdateMeetingMinute(meetingMinute);
            return res.Adapt<MeetingMinuteRes>();
        }

        public async System.Threading.Tasks.Task DeleteMeetingMinute(int id)
        {
            var meetMinu = await _repo.GetMeetingMinuteById(id);
            if (meetMinu == null)
            {
                throw new ValidationException("Meeting minute not found.");
            }
            await _repo.DeleteMeetingMinute(meetMinu);
        }

        public async Task<MeetingResponseDTO?> GetMeetingByIdAsync(int meetingId)
        {
            var meeting = await _repo.GetMeetingByIdAsync(meetingId);
            if (meeting == null)
                return null;

            return new MeetingResponseDTO
            {
                Id = meeting.Id,
                IsActive = meeting.IsActive,
                CreateAt = meeting.CreateAt,
                MeetingLink = meeting.MeetingLink,
                Time = meeting.Time,
                DayOfWeek = meeting.DayOfWeek,
                CreatedByName = meeting.CreateByNavigation?.Fullname
            };
        }

        public async Task<ApiResponse<List<MeetingScheduleDateDetailDto>>> GetMeetingScheduleDatesByGroupIdAsync(int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role == "Student")
            {
                var isInGroup = await _repo.CheckStudentInGroupAsync(user.Id ?? 0, groupId);
                if (!isInGroup)
                    throw new UnauthorizedAccessException("You are not a member of this group.");
            }
            else if (user.Role == "Mentor")
            {
                var isMentor = await _repo.CheckStudentInGroupAsync(user.Id ?? 0, groupId);
                if (!isMentor)
                    throw new UnauthorizedAccessException("You are not the mentor of this group.");
            }
            var list = await _repo.GetMeetingScheduleDatesByGroupIdAsync(groupId);

            if (list == null || list.Count == 0)
                return ApiResponse<List<MeetingScheduleDateDetailDto>>.Success(new List<MeetingScheduleDateDetailDto>(), "Không có ngày họp nào cho nhóm nàys.");

            var result = list.Select(msd => new MeetingScheduleDateDetailDto
            {
                Id = msd.Id,
                MeetingDate = msd.MeetingDate,
                Description = msd.Description,
                CreateAt = msd.Meeting?.CreateAt,
                MeetingLink = msd.Meeting?.MeetingLink,
                IsMeeting = msd.IsMeeting,
                Time = msd.Meeting?.Time,
                DayOfWeek = msd.Meeting?.DayOfWeek
            }).ToList();

            return ApiResponse<List<MeetingScheduleDateDetailDto>>.Success(result, "Lấy danh sách ngày họp thành công.");
        }

        public async Task<bool> UpdateIsMeetingAsync(int id, bool isMeeting)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role != "Mentor")
                throw new UnauthorizedAccessException("Only mentors can update meeting status.");

            var schedule = await _repo.GetByIdWithMeetingAndGroupsAsync(id);
            if (schedule == null)
                throw new ValidationException("Meeting schedule not found.");

            var mentorId = user.Id ?? 0;
            var mentorGroupIds = schedule.Meeting?.Groups?.Select(g => g.Id).ToList() ?? new List<int>();

            bool isMentorOfAnyGroup = false;
            foreach (var groupId in mentorGroupIds)
            {
                if (await _repo.CheckStudentInGroupAsync(mentorId, groupId))
                {
                    isMentorOfAnyGroup = true;
                    break;
                }
            }

            if (!isMentorOfAnyGroup)
                throw new UnauthorizedAccessException("You are not authorized to update this meeting schedule.");

            schedule.IsMeeting = isMeeting;
            schedule.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(schedule);
            return true;
        }

    }
}
