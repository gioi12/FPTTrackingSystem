using DataTranferObjects.Student.Meeting;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Student.Interfaces
{
    public interface IMeetingRepository
    {
        Task<GroupUser?> GetFreeTimeSlotAsync(int studentId, int groupId);
        Task<GroupUser> CreateFreeTimeSlotAsync(GroupUser entity);
        Task<GroupUser> UpdateFreeTimeSlotAsync(GroupUser entity);
        System.Threading.Tasks.Task SaveChangesAsync();
        Task<GroupFreeTimeDto> GetFreeTimeSlotsByGroupIdAsync(int groupId);
        Task<Meeting?> GetMeetingByGroupIdAsync(int groupId);
       /* Task<Meeting> FinalizeScheduleAsync(int groupId, FinalMeetingDto dto, int userId);*/

        Task<MeetingMinute> CreateMeetingMinute(MeetingMinute entity);

        Task<MeetingMinute?> GetMeetingMinuteByMeetingDate(int meetingDateId);
        Task<MeetingMinute?> GetMeetingMinuteById(int id);
        Task<MeetingMinute> UpdateMeetingMinute(MeetingMinute entity);
        System.Threading.Tasks.Task DeleteMeetingMinute(MeetingMinute entity);
        Task<Meeting?> GetMeetingByIdAsync(int meetingId);
        Task<Meeting> FinalizeOrUpdateScheduleAsync(int groupId, FinalMeetingDto dto, int userId);
        Task<MeetingScheduleDate?> GetMeetingDateByIdAsync(int id);
        System.Threading.Tasks.Task UpdateMeetingScheduleDate(MeetingScheduleDate m);
        Task<bool> CheckSecretary(int userId);
        Task<List<MeetingScheduleDate>> GetMeetingScheduleDatesByGroupIdAsync(int groupId);
        Task<MeetingScheduleDate?> GetByIdAsync(int id);
        Task<MeetingScheduleDate?> GetByIdWithMeetingAndGroupsAsync(int id);
        Task<bool> CheckStudentInGroupAsync(int studentId, int groupId);
        System.Threading.Tasks.Task UpdateAsync(MeetingScheduleDate entity);
        Task<List<UserSlot>> GetUserSlotsAsync(int userId, int groupId);
        System.Threading.Tasks.Task DeleteUserSlotsAsync(List<UserSlot> slots);
        System.Threading.Tasks.Task AddUserSlotsAsync(List<UserSlot> slots);
        Task<string> MeetingMinuteData(int groupId);
        Task<List<MeetingMinute>?> GetMeetingMinutesByMeetingId(int meetingId);
    }
}
