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
    public class MailRepository : IMailRepository
    {
        private readonly FpttrackingSystemContext _context;
        public MailRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async System.Threading.Tasks.Task NewMailSetting(MailSetting mail)
        {
            var current = await _context.MailSettings
                                       .FirstOrDefaultAsync(x =>(bool) x.IsActive);

            if (current != null)
            {
                current.IsActive = false;
            }

            _context.MailSettings.Add(mail);
             await _context.SaveChangesAsync();
        }
    }
}
