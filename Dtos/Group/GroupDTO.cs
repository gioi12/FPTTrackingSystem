using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Group
{
    public class GroupDto
    {
        public string Id { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public List<string> Supervisor { get; set; }
        public bool SubmittedDocs { get; set; }
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

    public class ApiResponse<T> {
        public int Status { get; set; } 
        public string Message { get; set; } = string.Empty; 
        public T? Data { get; set; } 
    }

    public class GroupDetailDto { 
        public string Id { get; set; } = string.Empty; 
        public string? ProjectName { get; set; }
        public List<string> Supervisors { get; set; } = new(); 
        public string? Status { get; set; } public string? Risk { get; set; } 
        public List<StudentDto> Students { get; set; } = new();
        public List<String>? ActivityLog { get; set; } = null;
    }

    public class StudentDto { 
        public string Id { get; set; } = string.Empty; 
        public string? Name { get; set; } 
        public string? Role { get; set; } 
    }

}
