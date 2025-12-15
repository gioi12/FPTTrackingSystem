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

        Task<List<Deliverable>> GetByCodeAndSemesterGroup(int code,int semesterId,int groupId);
        Task<List<Deliverable>> GetByCodeAndSemester(int code, int semesterId, int groupId);

        Task<Deliverable?> GetById(int id);

        Task<DeliveryItem?> GetItemByItemId(int id);
        Task<List<DeliverableGroup>> GetDeliverableGroupsByGroupIdAsync(int groupId);

    }
}
