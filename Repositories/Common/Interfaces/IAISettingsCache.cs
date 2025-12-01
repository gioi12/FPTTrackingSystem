using DataTranferObjects.Common.Request;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Interfaces
{
    public interface IAISettingsCache
    {
        Aisetting Settings { get; }
        System.Threading.Tasks.Task ReloadAsync();
    }
}
