using AutoMapper;
using IdentityProvider.Dtos;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface IRoleService
{
    public Task<IEnumerable<RoleDto>> RolesAsync();
    public Task<RoleDto> FindByIdAsync(string id);
    public Task<RoleDto> FindByNameAsync(string name);
    public Task<RoleDto?> CreateAsync(string name);
    public Task<bool> DeleteAsync(string id);
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
    public async Task<IEnumerable<RoleDto>> RolesAsync()
    {
        var roles = _roleManager.Roles?
            .Select(r => _mapper.Map<RoleDto>(r))
            .ToList()
            ?? Enumerable.Empty<RoleDto>();

        return await Task.FromResult(roles);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return false;

        await _roleManager.DeleteAsync(role);

        return true;
    }

    public async Task<RoleDto?> CreateAsync(string name)
    {
        var role = new IdentityRole(name);
        var result = await _roleManager.CreateAsync(role);

        return result.Succeeded ? _mapper.Map<RoleDto>(role) : null;
    }

    public async Task<RoleDto> FindByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> FindByNameAsync(string name)
    {
        var role = await _roleManager.FindByNameAsync(name);

        return _mapper.Map<RoleDto>(role);
    }
}