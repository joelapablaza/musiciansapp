using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Domain;

namespace API.Repositories
{
    public interface IAccountRepository
    {
        Task<AppUser> Register(string username, string password);
    }
}