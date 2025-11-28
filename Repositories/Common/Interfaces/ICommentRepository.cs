using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Interfaces
{
    public interface  ICommentRepository
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment?> GetCommentByIdAsync(int commentId);
        System.Threading.Tasks.Task DeleteCommentAsync(Comment comment);
        Task<Comment?> GetCommentAsync(int taskId, int commentId);
        System.Threading.Tasks.Task UpdateCommentAsync(Comment comment);
    }
}
