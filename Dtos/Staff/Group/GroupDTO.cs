using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Group
{
    public class GroupDto
    {
        public int? Id { get; set; }
        public string? GroupCode { get; set; } = string.Empty;
        public string? CourseCode { get; set; } = string.Empty;
        public string? Term { get; set; } = string.Empty;
        public string? Major { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool IsExpired { get; set; }
        public IEnumerable<string>? Supervisor { get; set; }
        public bool SubmittedDocs { get; set; }
    }

    public class GroupExpireDateDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Profession { get; set; }
        public string? Description { get; set; }
        public string? VietnameseTitle { get; set; }
    }

    public class UpdateExpireDateRequest
    {
        public DateTime ExpireDate { get; set; }
    }


    public class PagedResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PagedData<T> Data { get; set; } = new PagedData<T>();
    }

    public class PagedData<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Total { get; set; }
    }

    public class GroupMentorDto
    {
        public int Id { get; set; }
        public string? GroupCode { get; set; }
        public string? status { get; set; }
        public string? Name { get; set; }
        public bool IsExpired { get; set; }
        public List<StudentGroupDTO> students { get; set; } = new();
    }


    public class GroupDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string? GroupCode { get; set; }
        public string? ProjectName { get; set; }
        public int? SemesterId { get; set; }
        public bool IsExpired { get; set; }
        public DateTime? ExpireDate { get; set; }
        public List<string> Supervisors { get; set; } = new();
        public List<SuperviorDto> SupervisorsInfor { get; set; } = new();
        public string? Status { get; set; }
        public string? Risk { get; set; }
        public List<StudentDto> Students { get; set; } = new();
        public List<string>? ActivityLog { get; set; } = null;
    }

    public class SuperviorDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; } = string.Empty;
    }
    public class StudentGroupDTO { 
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string? Name { get; set; }
    }

    public class StudentDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Role { get; set; }
    }

    public class DashBoardGroupDto
    {
        public string name { get; set; } = string.Empty;
        public int Total { get; set; }
    }


    public class GroupTrackingResponseDto
    {
        public string CurrentWeek { get; set; } = string.Empty;
        public List<WeekDto> Weeks { get; set; } = new();
        public List<string> TimeSlots { get; set; } = new();
        public List<DayDto> Days { get; set; } = new();
        public List<GroupMemberDto> GroupMembers { get; set; } = new();
        public List<MilestoneDto> Milestones { get; set; } = new();
    }

    public class WeekDto
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class DayDto
    {
        public string Name { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public class GroupMemberDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsLeader { get; set; }
    }

    public class MilestoneDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
    }


}
