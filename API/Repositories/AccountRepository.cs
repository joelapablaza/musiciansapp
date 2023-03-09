using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountRepository(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<UserDTO> Register(RegisterDTO register)
        {
            var user = _mapper.Map<AppUser>(register);

            using var hmac = new HMACSHA512();

            user.UserName = register.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
                // Gender = "all",
            };
        }

        public async Task<UserDTO> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

            if (user == null)
            {
                return null;
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return null;
                }
            }

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
                // Gender = "all",
            };
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}