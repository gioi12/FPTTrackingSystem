using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Authentication
{
    public class RTRepository : IRTRepository
    {
        private readonly FpttrackingSystemContext _context;
        public RTRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateToken(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken?> FindByIdAndUserId(int userId,string hash)
        {
            return await _context.RefreshTokens
                       .FirstOrDefaultAsync(x =>
                           x.UserId == userId &&
                           x.Token == hash &&
                           x.IsRevoked == false &&
                           x.ExpireAt > DateTime.UtcNow);

        }

        public async Task<bool> RevokeToken(RefreshToken token)
        {
            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
