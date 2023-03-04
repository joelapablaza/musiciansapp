using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UsersRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        // GET USERS
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        // GET USER BY ID
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        // GET USER BY USERNAME
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        // SAVE ALL
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // UPDATE
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        // GET ALL APPUSERS DTOS
        public async Task<IEnumerable<AppUserDTO>> GetAppUserDTOsAsync()
        {
            return await _context.Users
                .ProjectTo<AppUserDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET APP USER DTO
        public async Task<AppUserDTO> GetAppUserDTOAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<AppUserDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }
    }
}