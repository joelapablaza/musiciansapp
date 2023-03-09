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
using System.Security.Claims;
using API.Extensions;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUsersRepository usersRepository, IMapper mapper,
        IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _usersRepository = usersRepository;

        }

        // GET ALL USERS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsersAsync([FromQuery] UserParams userParams)
        {
            var user = await _usersRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;

            // if (string.IsNullOrEmpty(userParams.Gender))
            // {
            //     userParams.Gender = user.Gender == "male" ? "female" : "male";
            // }

            var users = await _usersRepository.GetAppUserDTOsAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage,
                users.PageSize, users.TotalCount, users.TotalPages);

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
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<AppUser>> GetUser(string username)
        {
            var user = await _usersRepository.GetAppUserDTOAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserUpdateDTO userUpdateDTO)
        {
            var username = User.GetUsername();
            var user = await _usersRepository.GetUserByUsernameAsync(username);

            _mapper.Map(userUpdateDTO, user);
            _usersRepository.Update(user);

            if (await _usersRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Fail to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();
            var user = await _usersRepository.GetUserByUsernameAsync(username);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _usersRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> setMainPhoto(int photoId)
        {
            var user = await _usersRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _usersRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _usersRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _usersRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}