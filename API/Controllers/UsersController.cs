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
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            _mapper = mapper;
            _usersRepository = usersRepository;

        }

        // GET ALL USERS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsersAsync()
        {
            var users = await _usersRepository.GetAppUserDTOsAsync();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // [HttpGet("{id:int}")]
        // public async Task<ActionResult<AppUser>> GetUserByIdAsync(int id)
        // {
        //     var user = await _usersRepository.GetUserByIdAsync(id);

        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     var userDTO = _mapper.Map<AppUserDTO>(user);

        //     return Ok(userDTO);
        // }

        // GET USER BY USERNAME
        [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetUser(string username)
        {
            var user = await _usersRepository.GetAppUserDTOAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}