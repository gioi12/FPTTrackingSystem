using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Interfaces
{
    public interface IAttachmentRepository
    {
        System.Threading.Tasks.Task  AddAttachment(Attachment attachment);
        Task<List<Attachment>> GetAttachments(string entityName,int entityId,int groupId);

        Task<List<Attachment>> GetAttachmentsByIds(string entityName, List<int> entityIds, int groupId);

    }
}
