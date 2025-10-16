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
        System.Threading.Tasks.Task UpdateDeliverable(Deliverable delivery);

        Task<List<Deliverable>> GetByCodeAndSemester(int code,int semesterId);

    }
}
