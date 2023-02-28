using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace API.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<Entities.Domain.AppUser, Entities.DTO.AppUser>()
                .ReverseMap();
        }
    }
}