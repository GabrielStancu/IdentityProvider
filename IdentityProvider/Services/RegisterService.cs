using AutoMapper;
using IdentityProvider.Dtos;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface IAuthenticationService
{
    public Task<(UserDto? User, string Message)> RegisterAsync(RegisterDto registerDto);
    public Task<bool> DeleteUserAsync(DeleteDto deleteDto);
    public Task<bool> UserAlreadyExistsAsync(string email);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthenticationService(
        UserManager<IdentityUser> userManager,
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
        // Validate the request
        if(await UserAlreadyExistsAsync(registerDto.Email))
            return (null, "Email address is in use.");

        if (await RoleDoesNotExistAsync(registerDto.RoleId))
            return (null, "Assigned role does not exist.");

        // Create the role and validate it
        var role = await _roleManager.FindByIdAsync(registerDto.RoleId);
        if (role is null || string.IsNullOrEmpty(role.Name))
            return (null, "Invalid role");

        // Register the user
        var user = new IdentityUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email
        };
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            return (null, string.Join(';', result.Errors));

        // Assign the user to the role
        var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
        if (!roleResult.Succeeded)
            return (null, string.Join(';', roleResult.Errors));

        // Create return object
        var registeredUser = new UserDto
        {
            Token = _tokenService.CreateToken(user, registerDto.RoleId),
            Email = user.Email,
            Role = _mapper.Map<RoleDto>(role)
        };

        return (registeredUser, string.Empty);
    }
    public async Task<bool> DeleteUserAsync(DeleteDto deleteDto)
    {
        var user = await _userManager.FindByEmailAsync(deleteDto.Email);
        if (user is null)
            return false;

        var isValidPassword = await _userManager.CheckPasswordAsync(user, deleteDto.Password);
        if (!isValidPassword)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UserAlreadyExistsAsync(string email)
        => await _userManager.FindByEmailAsync(email) != null;

    private async Task<bool> RoleDoesNotExistAsync(string roleId)
        => await _roleManager.FindByIdAsync(roleId) is null;
}