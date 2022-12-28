using IdentityProvider.Dtos;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public interface ILoginService
{
    public Task<UserDto?> LoginAsync(LoginDto loginDto);
}

public class LoginService : ILoginService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
            return null;

        return new UserDto
        {
            Email = user.Email!,
            //TODO: get the role for the "?" placeholder below
            Token = _tokenService.CreateToken(user, "?")
        };
    }
}
