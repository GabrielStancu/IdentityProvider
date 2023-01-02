using AutoMapper;
using IdentityProvider.Dtos;
using IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface IRegisterService
{
    public Task<(UserDto? User, string Message)> RegisterAsync(RegisterDto registerDto);
}

public class RegisterService : IRegisterService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public RegisterService(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<(UserDto? User, string Message)> RegisterAsync(RegisterDto registerDto)
    {
        // If client does not support UserNames, it is set as the Email address
        if (string.IsNullOrEmpty(registerDto.UserName))
        {
            registerDto.UserName = registerDto.Email;
        }

        // Validate the request
        if(await EmailAlreadyInUseAsync(registerDto.Email))
            return (null, "Email address is in use.");

        if(await UserNameAlreadyInUseAsync(registerDto.UserName))
            return (null, "UserName is in use.");

        if (await RoleDoesNotExistAsync(registerDto.RoleId))
            return (null, "Assigned role does not exist.");

        // Create the role and validate it
        var role = await _roleManager.FindByIdAsync(registerDto.RoleId);
        if (role is null || string.IsNullOrEmpty(role.Name))
            return (null, "Invalid role");

        // Register the user
        var user = _mapper.Map<AppUser>(registerDto);
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            return (null, string.Join(';', result.Errors));

        // Assign the user to the role
        var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
        if (!roleResult.Succeeded)
            return (null, string.Join(';', roleResult.Errors));

        // Create return object
        var registeredUser = _mapper.Map<UserDto>(user);
        registeredUser.Token = _tokenService.CreateToken(user, registerDto.RoleId);
        registeredUser.Role = _mapper.Map<RoleDto>(role);

        return (registeredUser, string.Empty);
    }

    private async Task<bool> EmailAlreadyInUseAsync(string email)
        => await _userManager.FindByEmailAsync(email) != null;

    private async Task<bool> UserNameAlreadyInUseAsync(string userName)
        => await _userManager.FindByNameAsync(userName) != null;

    private async Task<bool> RoleDoesNotExistAsync(string roleId)
        => await _roleManager.FindByIdAsync(roleId) is null;
}