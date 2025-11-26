using DataTranferObjects.Common.Request;
using Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Implements
{
    public class MailSettingCache : IMailSettingCache
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MailSettings Settings { get; private set; }

        public MailSettingCache(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        async System.Threading.Tasks.Task IMailSettingCache.ReloadAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FpttrackingSystemContext>();

            var setting = context.MailSettings.FirstOrDefault(x => x.IsActive == true);
            if (setting == null)
            {
                throw new Exception("Mail settings not found in the database.");
            }
            Settings = new MailSettings
            {
                Mail = setting.Mail,
                DisplayName = setting.DisplayName,
                Password = setting.Password,
                Host = setting.Host,
                Port = (int)setting.Port,
            };
        }
    }
}
