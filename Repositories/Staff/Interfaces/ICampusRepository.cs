using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Interfaces
{
    public interface ICampusRepository
    {
        Task<IEnumerable<Campus>> GetAllCampusesAsync();
        Task<Campus?> GetByIdWithSlotsAsync(int id);
        Task<Slot> AddSlotAsync(int campusId, Slot slot);
        Task<Slot?> UpdateSlotAsync(int campusId, Slot slot);
        Task<bool> DeleteSlotAsync(int campusId, int slotId);
    }
}
