using DataTranferObjects.Staff.Response;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Interfaces
{
    public interface IMilestoneRepository
    {
        Task<List<Milestone>> NewMilestontes(List<Milestone> list, int majorId);
        Task<List<Milestone>> UpdateMilestonte(Milestone milestone, int majorId);
        Task<List<Milestone>> DeleteMilestone(int id);
        Task<List<Milestone>> GetByMajor(int id);
        Task<Milestone?> GetMilestone(int id);
    }
}
