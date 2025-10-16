using Entities.Models;
using Repositories.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Implements
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly FpttrackingSystemContext _context;
        public AttachmentRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async System.Threading.Tasks.Task AddAttachment(Attachment attachment)
        {
            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }
    }
}
