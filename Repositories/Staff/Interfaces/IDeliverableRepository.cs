using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Interfaces
{
    public interface IDeliverableRepository
    {
        Task<Deliverable?> GetByMileIdAndActiveSenmester(int mileId);
        void UpdateDeliverable(Deliverable delivery);
    }
}
