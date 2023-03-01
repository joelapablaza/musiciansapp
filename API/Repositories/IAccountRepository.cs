using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;

namespace API.Repositories
{
    public interface IAccountRepository
    {
        Task<AppUser> Register(RegisterDTO register);
        Task<bool> UserExists(string username);
        Task<AppUser> Login(LoginDTO loginDTO);
    }
}