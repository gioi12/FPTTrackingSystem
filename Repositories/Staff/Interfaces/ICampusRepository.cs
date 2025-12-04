using DataTranferObjects.Staff.Campus;
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
        Task<Slot?> GetByIdAsync(int id);
        System.Threading.Tasks.Task UpdateAsync(Slot slot);
        Task<Campus> AddCampusAsync(Campus campus);
        Task<Campus?> GetCampusByIdAsync(int id);
        System.Threading.Tasks.Task UpdateCampusAsync(Campus campus);

        Task<Campus?> GetCampusWithSlotsAsync(int campusId);
        System.Threading.Tasks.Task AddSlotsAsync(List<Slot> slots);
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}
