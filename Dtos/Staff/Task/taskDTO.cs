using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Task
{
    public class taskDTO
    {
    }
    public class CreateTaskDTO
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string TaskType { get; set; } = null!;
        public DateTime EndAt { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int? MeetingId { get; set; }
        public int? DeliverableId { get; set; }
        public int? AssignedUserId { get; set; }
        public int? ReviewerId { get; set; }
    }

    public class TaskStatisticResponse
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int UncompletedTasks { get; set; }
    }


    public class UpdateTaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int GroupId { get; set; }
        public string? Description { get; set; }
        public DateTime EndAt { get; set; }
        public string StatusId { get; set; }
        public string PriorityId { get; set; }
        public string? Process { get; set; }
        public int? DeliverableId { get; set; }
        public int? MeetingId { get; set; }
        public int AssignedUserId { get; set; }
        public int? ReviewerId { get; set; }
    }

    public class TaskReviewerDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public string? Type { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string? Priority { get; set; }
    }



    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? EndAt { get; set; }

        public DateTime Deadline { get; set; }
        public int GroupId { get; set; }

        public int? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
    }

    public class TaskResponseUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string? StatusId { get; set; }
        public string? PriorityId { get; set; }
        public string? Process { get; set; }
        public int? MilestoneId { get; set; }
        public int? MeetingId { get; set; }
        public int? GroupId { get; set; }
        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        public int? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
    }


    public class TaskDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool isMeetingTask { get; set; }
        public bool isActive { get; set; }
        public int meetingId { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }

        public int? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public int? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public string? TaskType { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }

        public GroupTaskDto? Group { get; set; }

        public MilestonesDto? Milestone { get; set; }

        public List<AttachmentDto>? Attachments { get; set; }

        public List<CommentDto>? Comments { get; set; }

        public List<HistoryDto>? History { get; set; }
    }
    public class TaskResponsesDto
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime Deadline { get; set; }

        public string? Type { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public string? Priority { get; set; }

        public bool? IsActive { get; set; }
    }


    public class GroupTaskDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class MilestonesDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool? isActive { get; set; }
        public string? Description { get; set; }
    }

    public class AttachmentDto
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Author { get; set; }          
        public string AuthorName { get; set; }     
        public string Content { get; set; }         
        public DateTime Timestamp { get; set; }    
    }

    public class HistoryDto
    {
        public int Id { get; set; }          
        public string Detail { get; set; }          
        public DateTime At { get; set; }           
        public string User { get; set; }           
        public string Action { get; set; }          
    }


}
