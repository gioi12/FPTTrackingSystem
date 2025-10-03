using DataTranferObjects.Staff.Response;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff
{
    public interface IMilestoneRepository 
    {
        Task<List<Milestone>> NewMilestontes(List<Milestone> list);
        Task<List<Milestone>> GetByMajorAndSemester(int major, int semester);
        Task<List<Milestone>> updateMilestontes(List<Milestone> list);
        Task<List<Milestone>> deleteMilestone(int id);

    }
}
