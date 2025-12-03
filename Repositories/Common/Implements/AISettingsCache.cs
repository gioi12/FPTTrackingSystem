using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Implements
{
    public class AISettingsCache : IAISettingsCache
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public Aisetting Settings{ get; private set; }

        public AISettingsCache(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async System.Threading.Tasks.Task ReloadAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FpttrackingSystemContext>();

            var setting = await context.Aisettings
                                       .FirstOrDefaultAsync(x => x.IsActive == true);

            if (setting == null)
                throw new Exception("AI settings not found in the database.");

            Settings = new Aisetting
            {
                Id = setting.Id,
                Name = setting.Name,
                SecretKey = setting.SecretKey,
                IsActive = setting.IsActive
            };
        }
    }
}
