using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Implements
{
    public class CommentRepository: ICommentRepository

    {
        private readonly FpttrackingSystemContext _context;
        public CommentRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            return await _context.Comments
                .FirstOrDefaultAsync(x => x.Id == commentId);
        }

        public async System.Threading.Tasks.Task DeleteCommentAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
