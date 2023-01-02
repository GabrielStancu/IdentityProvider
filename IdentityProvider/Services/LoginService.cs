using AutoMapper;
using IdentityProvider.Dtos;
using IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface ILoginService
{
    public Task<UserDto?> LoginAsync(LoginDto loginDto);
}

public class LoginService : ILoginService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public LoginService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        // Check the user exists
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            return null;

        // Check the password
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
            return null;

        // Get the user role name and validate it
        var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (string.IsNullOrEmpty(roleName))
            return null;

        // Get the user role and validate it
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null || string.IsNullOrEmpty(role.Name))
            return null;

        // Create the return user object
        var loggedUser = _mapper.Map<UserDto>(user);
        loggedUser.Token = _tokenService.CreateToken(user, role.Name);
        loggedUser.Role = _mapper.Map<RoleDto>(role);

        return loggedUser;
    }
}
