using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Repositories;
using AutoMapper;
using API.DTO;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            _mapper = mapper;
            _usersRepository = usersRepository;

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _usersRepository.GetUsers();

            if (users == null)
            {
                return NotFound();
            }

            var usersDTO = _mapper.Map<List<AppUserDTO>>(users);

            return Ok(usersDTO);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUser>> GetUserAsync(int id)
        {
            var user = await _usersRepository.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDTO = _mapper.Map<AppUserDTO>(user);

            return Ok(userDTO);
        }
    }
}