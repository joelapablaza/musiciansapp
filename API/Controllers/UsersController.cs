using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    // public class UsersController : ControllerBase
    // {
    //     private readonly DataContext _context;
    //     public UsersController(DataContext context)
    //     {
    //         _context = context;
    //     }

    //     [HttpGet]
    //     public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() // En este caso se podria usar <List> pero solamente necesitamos una colecion iterable, por eso usamos IEnumarable
    //     {
    //         return await _context.Users.ToListAsync();
    //     }

    //     [HttpGet("{id}")]
    //     public ActionResult<AppUser> GetUser(int id)
    //     {
    //         return _context.Users.Find(id);
    //     }
    // }

    [ApiController]
    [Route("api/")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("users/{id:int}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}