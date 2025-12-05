using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Major;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Interfaces
{
    public interface IMajorRepository
    {
        Task<List<Major>> findAll();
        Task<List<Major>> getAllMajorAndCode();
        Task<PagedData<MajorCategory>> GetAllCoursePagedAsync(int page, int pageSize);
        Task<List<MajorCategory>> getAllCourse();
        Task<MajorCategory?> GetByIdAsync(int id);
        Task<MajorCategory?> FindByCodeAsync(string code);
        Task<bool> CreateAsync(MajorCategory majorCategory);
        Task<bool> UpdateAsync(MajorCategory majorCategory);
    }
}
