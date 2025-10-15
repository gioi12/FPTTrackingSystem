using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Interfaces
{
    public interface ISemesterRepository
    {
        Task<Semester?> findActive();
        Task<List<Semester>> getAllSemesters();
        Task<Semester?> GetSemesterByIdAsync(int id);
        Task<Semester?> GetSemesterByNow();
        Task<Semester?> GetMilestonesBySemester(int id);
        Task<Semester?> GetDeliveriesBySemester(int id);
        Task<Semester?> GetGroupsBySemester(int id);
    }
}
