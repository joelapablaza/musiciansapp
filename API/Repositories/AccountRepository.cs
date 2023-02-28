using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities.Domain;

namespace API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext _context;
        public AccountRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<AppUser> Register(string username, string password)
        {
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}