using AutoMapper;
using IdentityProvider.Dtos;
using IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IdentityRole, RoleDto>();
        CreateMap<AppUser, UserDto>();
        CreateMap<AppUser, UserInfoDto>();
    }
}