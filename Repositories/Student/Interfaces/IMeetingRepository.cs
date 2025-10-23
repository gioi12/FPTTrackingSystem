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
        Task<List<StudentFreeTimeDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId);
        Task<Meeting?> GetMeetingByGroupIdAsync(int groupId);
        Task<Meeting> FinalizeScheduleAsync(int groupId, FinalMeetingDto dto, int userId);

        Task<MeetingMinute> CreateMeetingMinute(MeetingMinute entity);

        Task<MeetingMinute?> GetMeetingMinuteByMeeting(int meetingId);
        Task<MeetingMinute?> GetMeetingMinuteById(int id);
        Task<MeetingMinute> UpdateMeetingMinute(MeetingMinute entity);
        System.Threading.Tasks.Task DeleteMeetingMinute(MeetingMinute entity);

    }
}
