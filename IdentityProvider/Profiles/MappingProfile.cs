using AutoMapper;
using IdentityProvider.Dtos;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IdentityRole, RoleDto>();
    }
}