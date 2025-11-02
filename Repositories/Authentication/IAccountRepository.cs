using DataTranferObjects.Login;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Authentication
{
    public interface IAccountRepository
    {
        public Task<Account?> LoginAsync(LoginDTO loginDto);
        public Task<UserInfo?> UserInfo(int id);
        Task<Semester?> GetSemesterByNow();

        public Task<List<Account>> CreateUsers(List<Account> accounts);
    }
}
