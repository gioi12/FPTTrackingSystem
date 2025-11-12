using DataTranferObjects.Enum;
using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Repositories.Student.Implements;
using Repositories.Student.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;

namespace FPTTrackingSystem.Services.Student.Implements
{
    public class MeetingService : IMeetingService
    {
        private readonly IMeetingRepository _repo;
        private readonly AuthUtils _authUtils;
        private readonly FpttrackingSystemContext _context;

        public MeetingService(IMeetingRepository repo, AuthUtils authUtils, FpttrackingSystemContext context)
        {
            _repo = repo;
            _authUtils = authUtils;
            _context = context;
        }

        public async Task<object> CreateOrUpdateFreeTimeSlotsAsync(int groupId, FreeTimeSlotsRequest request)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                throw new UnauthorizedAccessException("User not logged in.");

            if (user.Role != "Student")
                throw new UnauthorizedAccessException("Only students are allowed to update free time slots.");

            var isInGroup = await _repo.CheckStudentInGroupAsync(user.Id ?? 0, groupId);
            if (!isInGroup)
                throw new UnauthorizedAccessException("You are not a member of this group.");

            if (request.FreeTimeSlots == null || !request.FreeTimeSlots.Any())
                throw new Exception("FreeTimeSlots cannot be empty.");

            var now = DateTime.UtcNow;
            var studentId = request.FreeTimeSlots.First().StudentId;

            var dayOfWeeks = request.FreeTimeSlots
                .Select(x => CapitalizeFirstLetter(x.DayOfWeek))
                .Distinct()
                .ToList();

            var timesMatrix = request.FreeTimeSlots
                .Select(day => day.TimeSlots.SelectMany(t => t.TimeSlots).ToList())
                .ToList();

            var serializedFreeTime = JsonSerializer.Serialize(timesMatrix);
            var joinedDays = string.Join(", ", dayOfWeeks);

            var existing = await _repo.GetFreeTimeSlotAsync(studentId, groupId);

            if (existing == null)
            {
                var newSlot = new GroupUser
                {
                    UserId = studentId,
                    GroupId = groupId,
                 /*   DayOfWeek = joinedDays,*/
                    CreateAt = now,
                    UpdateAt = now,
                    IsActive = true,
                    Role = "Student"
                };

                await _repo.CreateFreeTimeSlotAsync(newSlot);
            }
            else
            {
             /*   existing.DayOfWeek = joinedDays;*/
                existing.UpdateAt = now;

                await _repo.UpdateFreeTimeSlotAsync(existing);
            }

            await _repo.SaveChangesAsync();

            return new
            {
                success = true,
                message = "Free time slots created or updated successfully.",
                data = new
                {
                    studentId = studentId,
                    groupId = groupId,
                    dayOfWeek = joinedDays,
                    freeTime = serializedFreeTime,
                    updatedAt = now
                }
            };
        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public async Task<GroupFreeTimeDto> GetFreeTimeSlotsByGroupIdAsync(int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            // Kiểm tra role nếu là Student
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

            if (user.Role != "Supervisor")
                throw new UnauthorizedAccessException("Only mentors are allowed to finalize meeting schedules.");

            var isMentorOfGroup = await _repo.CheckStudentInGroupAsync(user.Id ?? 0, groupId);
            if (!isMentorOfGroup)
                throw new UnauthorizedAccessException("You are not the mentor of this group.");
            var meeting = await _repo.FinalizeOrUpdateScheduleAsync(groupId, dto.FinalMeeting, user.Id ?? 0);

            var slot = meeting.Slot ?? await _context.Slots.FindAsync(meeting.SlotId);
            return new FinalizeScheduleResponseDto
            {
                FinalMeeting = new FinalMeetingInfo
                {
                    Id = meeting.Id,
                    IsFinalized = meeting.IsActive ?? false,
                    Day = meeting.DayOfWeek ?? string.Empty,
                    /*Time = meeting.Time ?? string.Empty,*/  // sửa time này bằng slot id và response đầy đủ thông tin của slots
                    MeetingLink = meeting.MeetingLink ?? string.Empty,
                    FinalizedAt = meeting.CreateAt ?? DateTime.UtcNow,
                    UpdatedAt = meeting.UpdateAt ?? DateTime.UtcNow,
                     Slot = new SlotInfo
                     {
                         Id = slot?.Id ?? 0,
                         NameSlot = slot?.NameSlot ?? string.Empty,
                         StartAt = slot?.StartAt,
                         EndAt = slot?.EndAt
                     }
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

        public async Task<MeetingResponseDTO?> GetMeetingByGroupIdAsync(int GroupId)
        {
            var meeting = await _repo.GetMeetingByGroupIdAsync(GroupId);
            if (meeting == null)
                return null;

            return new MeetingResponseDTO
            {
                Id = meeting.Id,
                IsActive = meeting.IsActive,
                CreateAt = meeting.CreateAt,
                MeetingLink = meeting.MeetingLink,
                DayOfWeek = meeting.DayOfWeek,
                CreatedByName = meeting.CreateByNavigation?.Fullname,
                Slot = meeting.Slot == null ? null : new SlotInfo
                {
                    Id = meeting.Slot.Id,
                    NameSlot = meeting.Slot.NameSlot,
                    StartAt = meeting.Slot.StartAt,
                    EndAt = meeting.Slot.EndAt,
                }
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
            else if (user.Role == "Supervior")
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
                SlotInfor = new SlotInfo
                {
                    Id = msd.Slot.Id,
                   NameSlot = msd.Slot.NameSlot,
                   StartAt = msd.Slot.StartAt,
                   EndAt = msd.Slot.EndAt
                },
                DayOfWeek = msd.Meeting?.DayOfWeek,
                IsMinute = msd.MeetingMinute != null ? true : false
            }).ToList();

            return ApiResponse<List<MeetingScheduleDateDetailDto>>.Success(result, "Lấy danh sách ngày họp thành công.");
        }

        public async Task<ApiResponse<MeetingScheduleDateDetailDto>> UpdateMeetingScheduleDateAsync(int id, UpdateMeetingScheduleDateDto dto)
        {
            var msd = await _repo.GetByIdAsync(id);
            if (msd == null)
                throw new Exception("Buổi họp không tồn tại hoặc đã bị xóa.");

            var now = DateTime.Now;

            if (msd.MeetingDate <= now)
                throw new Exception("Bạn không thể cập nhật buổi họp đã diễn ra.");

            if (dto.MeetingDate.HasValue)
            {
                var newDate = dto.MeetingDate.Value;

                if (newDate <= now)
                    throw new Exception("Ngày mới phải lớn hơn hiện tại.");

                var oldWeek = ISOWeek.GetWeekOfYear(msd.MeetingDate ?? now);
                var newWeek = ISOWeek.GetWeekOfYear(newDate);
                var oldYear = (msd.MeetingDate ?? now).Year;
                var newYear = newDate.Year;

                if (oldWeek != newWeek || oldYear != newYear)
                    throw new Exception("Ngày họp phải nằm trong cùng tuần với ngày hiện tại.");
            }
            msd.MeetingDate = dto.MeetingDate ?? msd.MeetingDate;
            msd.SlotId = dto.SlotId ?? msd.SlotId;
            msd.IsActive = dto.IsActive ?? msd.IsActive;
            msd.Description = dto.Description ?? msd.Description;
            msd.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(msd);

            var result = new MeetingScheduleDateDetailDto
            {
                Id = msd.Id,
                MeetingDate = msd.MeetingDate,
                Description = msd.Description,
                CreateAt = msd.Meeting?.CreateAt,
                MeetingLink = msd.Meeting?.MeetingLink,
                DayOfWeek = msd.Meeting?.DayOfWeek,
                IsMeeting = msd.IsMeeting,
                IsActive = msd.IsActive,
                IsMinute = msd.MeetingMinute != null,
                SlotInfor = msd.Slot == null ? null : new SlotInfo
                {
                    Id = msd.Slot.Id,
                    NameSlot = msd.Slot.NameSlot,
                    StartAt = msd.Slot.StartAt,
                    EndAt = msd.Slot.EndAt
                }
            };

            return ApiResponse<MeetingScheduleDateDetailDto>.Success(result, "Cập nhật buổi họp thành công.");
        }

        public async System.Threading.Tasks.Task CreateOrUpdateFreeTimeSlotsAsync(int groupId, List<FreeTimeSlotRequest> requests)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            // 1. Lấy dữ liệu cũ
            var existing = await _repo.GetUserSlotsAsync(user.Id ?? 0, groupId);

            // 2. Xóa hết dữ liệu cũ
            if (existing.Any())
            {
                await _repo.DeleteUserSlotsAsync(existing);
            }

            // 3. Tạo danh sách mới
            var newSlots = requests.Select(r => new UserSlot
            {
                UserId = user.Id ?? 0,
                GroupId = groupId,
                SlotId = r.SlotId,
                DayOfWeek = r.DayOfWeek,
                CreateAt = DateTime.UtcNow
            }).ToList();

            // 4. Thêm mới
            await _repo.AddUserSlotsAsync(newSlots);
        }

        public async Task<bool> UpdateIsMeetingAsync(int id, bool isMeeting)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.RoleInGroup != "Secretary")
                throw new UnauthorizedAccessException("Only mentors or secretary can update meeting status.");

            var schedule = await _repo.GetByIdWithMeetingAndGroupsAsync(id);
            if (schedule == null)
                throw new ValidationException("Meeting schedule not found.");

            if (schedule.MeetingDate <= DateTime.Now)
                throw new ValidationException("You cannot update the status of a meeting that has already occurred.");

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
