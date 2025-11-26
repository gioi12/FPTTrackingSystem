using DataTranferObjects.Common.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Interfaces
{
    public interface IMailSettingCache
    {
        MailSettings Settings { get; }
        Task ReloadAsync();
    }
}
