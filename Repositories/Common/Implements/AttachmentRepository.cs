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

        public async System.Threading.Tasks.Task DeleteAttachment(Attachment attachment)
        {
            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task<Attachment?> GetAttachmentById(int attachmentId)
        {
            return  await _context.Attachments.FirstOrDefaultAsync(x => x.Id == attachmentId);
        }

        public async Task<List<Attachment>> GetAttachments(string entityName, int entityId, int groupId)
        {
           return await _context.Attachments
                .Include(x=>x.User)
                .Where(x=> x.EntityName == entityName && x.EntityId == entityId && x.GroupId == groupId )
                .ToListAsync();
        }

        public async Task<List<Attachment>> GetAttachmentsByIds(string entityName, List<int> entityIds, int groupId)
        {
            if (entityIds == null || entityIds.Count == 0)
                return new List<Attachment>();

            return await _context.Attachments
                .Where(x => x.EntityName == entityName
                            && entityIds.Contains(x.EntityId)
                            && x.GroupId == groupId)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task UpdateAttachment(Attachment attachment)
        {
             _context.Attachments.Update(attachment);
             await _context.SaveChangesAsync();
        }
    }
}
