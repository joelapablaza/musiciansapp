using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Helpers;
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

        // UPDATE
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        // GET ALL APPUSERS DTOS
        public async Task<PagedList<AppUserDTO>> GetAppUserDTOsAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUsername);

            if (userParams.Gender != "all")
            {
                query = query.Where(x => x.Gender == userParams.Gender);
            }

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<AppUserDTO>.CreateAsync(query.ProjectTo<AppUserDTO>(_mapper
                .ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber, userParams.PageSize);
        }

        // GET APP USER DTO
        public async Task<AppUserDTO> GetAppUserDTOAsync(string username, bool? isCurrentUser)
        {
            var query = _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<AppUserDTO>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if ((bool)isCurrentUser) query = query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
                    .Where(x => x.UserName == username)
                    .Select(x => x.Gender)
                    .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByPhotoId(int photoId)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .IgnoreQueryFilters()
            .Where(p => p.Photos.Any(p => p.Id == photoId))
            .FirstOrDefaultAsync();
        }
    }
}