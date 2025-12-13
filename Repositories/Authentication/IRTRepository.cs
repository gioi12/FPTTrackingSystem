using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Authentication
{
    public interface IRTRepository
    {
        Task<bool> CreateToken(RefreshToken token);
        Task<RefreshToken?> FindByIdAndUserId(int userId,string hash);

        Task<bool> RevokeToken(RefreshToken token);
    }
}
