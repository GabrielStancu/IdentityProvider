using AutoMapper;
using IdentityProvider.Dtos;
using IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface IUserService
{
    public Task<IEnumerable<UserInfoDto>> GetUsersAsync();
    public Task<IEnumerable<UserInfoDto>> GetUsersInRoleAsync(string roleName);
    public Task<bool> DeleteUserAsync(DeleteDto deleteDto);
    public Task<bool> UserAlreadyExistsAsync(string userName);
}

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserInfoDto>> GetUsersAsync()
    {
        var users = _mapper.Map<IEnumerable<UserInfoDto>>(_userManager.Users)
            .Select(async u => { u.RoleName = await UserRoleAsync(u.Id); return u; })
            .Select(t => t.Result);

        return await Task.FromResult(users);
    }

    public async Task<IEnumerable<UserInfoDto>> GetUsersInRoleAsync(string roleName)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

        return _mapper.Map<IEnumerable<UserInfoDto>>(usersInRole)
            .Select(u => { u.RoleName = roleName; return u; });
    }

    public async Task<bool> DeleteUserAsync(DeleteDto deleteDto)
    {
        var user = await _userManager.FindByNameAsync(deleteDto.UserName);
        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(deleteDto.UserName);
            if (user is null)
            {
                Console.WriteLine($"--> User {deleteDto.UserName} does not exist.");
                return false;
            }
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, deleteDto.Password);
        if (!isValidPassword)
        {
            Console.WriteLine($"--> Invalid credentials for deleting the user {deleteDto.UserName}");
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UserAlreadyExistsAsync(string userName)
        => await _userManager.FindByNameAsync(userName) != null ||
           await _userManager.FindByEmailAsync(userName) != null;

    private async Task<string> UserRoleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return string.Empty;

        return (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
    }
}