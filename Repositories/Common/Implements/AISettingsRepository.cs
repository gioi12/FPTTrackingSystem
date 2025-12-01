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
    public class AISettingsRepository : IAISettingsRepository
    {
        private readonly FpttrackingSystemContext _context;
        public AISettingsRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<Aisetting> GetSettings()
        {
            return await _context.Aisettings.FirstOrDefaultAsync(s => s.IsActive == true);
        }

        public async Task<bool> NewSettings(Aisetting setting)
        {
            var activeSetting = await _context.Aisettings.FirstOrDefaultAsync(s => s.IsActive == true);
            if (activeSetting != null)
            {
                activeSetting.IsActive = false;
                _context.Aisettings.Update(activeSetting);
            }
            setting.IsActive = true;
            await _context.Aisettings.AddAsync(setting);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
