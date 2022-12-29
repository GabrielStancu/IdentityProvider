using AutoMapper;
using IdentityProvider.Dtos;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface IRoleService
{
    public Task<IEnumerable<RoleDto>> AllRoles();
    public Task<RoleDto> RoleById(string id);
    public Task<RoleDto> RoleByName(string name);
    public Task<RoleDto?> Register(string name);
    public Task<bool> DeleteRole(string id);
}

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }
    public async Task<IEnumerable<RoleDto>> AllRoles()
    {
        var roles = _roleManager.Roles?
            .Select(r => _mapper.Map<RoleDto>(r))
            .ToList()
            ?? Enumerable.Empty<RoleDto>();

        return await Task.FromResult(roles);
    }

    public async Task<bool> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return false;

        await _roleManager.DeleteAsync(role);

        return true;
    }

    public async Task<RoleDto?> Register(string name)
    {
        var role = new IdentityRole(name);
        var result = await _roleManager.CreateAsync(role);

        return result.Succeeded ? _mapper.Map<RoleDto>(role) : null;
    }

    public async Task<RoleDto> RoleById(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> RoleByName(string name)
    {
        var role = await _roleManager.FindByNameAsync(name);

        return _mapper.Map<RoleDto>(role);
    }
}