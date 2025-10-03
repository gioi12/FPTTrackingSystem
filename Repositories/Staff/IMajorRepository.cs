using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff
{
    public interface IMajorRepository
    {
        Task<List<Major>> findAll();
    }
}
