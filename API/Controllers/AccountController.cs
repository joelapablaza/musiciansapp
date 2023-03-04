using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountRepository accountRepository, ITokenService tokenService)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUserDTO>> Register(RegisterDTO register)
        {
            var userIsTaken = await _accountRepository.UserExists(register.Username);

            if (userIsTaken)
            {
                return BadRequest("Username already exists");
            }

            var user = await _accountRepository.Register(register);

            return new AppUserDTO
            {
                Username = user.UserName,
                // Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _accountRepository.Login(loginDTO);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return new AppUserDTO
            {
                Username = user.UserName,
                // Token = _tokenService.CreateToken(user)
            };
        }
    }
}