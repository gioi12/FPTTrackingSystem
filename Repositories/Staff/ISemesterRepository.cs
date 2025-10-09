using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff
{
    public interface ISemesterRepository
    {
        Task<Semester?> findActive();
        Task<List<Semester>> getAllSemesters();
        Task<Semester?> GetSemesterByIdAsync(int id);
    }
}
