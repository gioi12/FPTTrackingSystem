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
    }
}
