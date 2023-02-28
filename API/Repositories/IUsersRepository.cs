using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Domain;

namespace API.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<AppUser>> GetUsers();
        Task<AppUser> GetUser(int id);
    }
}