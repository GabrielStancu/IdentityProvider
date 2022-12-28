using IdentityProvider.Dtos;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface IRegisterService
{
    public Task<(UserDto? User, string Message)> RegisterAsync(RegisterDto registerDto);
    public Task<bool> UserAlreadyExistsAsync(string email);
}

public class RegisterService : IRegisterService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterService(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<(UserDto? User, string Message)> RegisterAsync(RegisterDto registerDto)
    {
        if(await UserAlreadyExistsAsync(registerDto.Email))
        {
            return (null, "Email address is in use.");
        }

        var user = new IdentityUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            return (null, "Invalid credentials");

        var registeredUser = new UserDto
        {
            Token = _tokenService.CreateToken(user, registerDto.UserRole),
            Email = user.Email
        };

        return (registeredUser, string.Empty);
    }

    public async Task<bool> UserAlreadyExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}