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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
         ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<UserDTO> Register(RegisterDTO register)
        {
            var user = _mapper.Map<AppUser>(register);

            user.UserName = register.Username.ToLower();


            var result = await _userManager.CreateAsync(user, register.Password);

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
                // Gender = "all",
            };

            if (!result.Succeeded) return null;
        }

        public async Task<UserDTO> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if (!result.Succeeded) return null;

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
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}