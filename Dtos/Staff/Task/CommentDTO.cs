using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Task
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }

    public class CreateCommentDto
    {
        public int TaskId { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }
}
