using IdentityProvider.Dtos;
using IdentityProvider.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IAuthenticationService _authenticationService;

    public AccountController(ILoginService loginService, IAuthenticationService authenticationService)
    {
        _loginService = loginService;
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        Console.WriteLine($"--> Authenticating user {loginDto.Email}...");

        var user = await _loginService.LoginAsync(loginDto);

        return user ?? (ActionResult<UserDto>)Unauthorized();
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        Console.WriteLine($"--> Registering user {registerDto.Email}...");

        var registerResult = await _authenticationService.RegisterAsync(registerDto);

        return registerResult.User ?? (ActionResult<UserDto>)BadRequest(registerResult.Message);
    }

    [HttpPost("delete")]
    public async Task<ActionResult> Delete(DeleteDto deleteDto)
    {
        Console.WriteLine($"--> Deleting user {deleteDto.Email}...");

        var deleted = await _authenticationService.DeleteUserAsync(deleteDto);

        return deleted ? Ok() : BadRequest();
    }

    [HttpPost("exists/{email}")]
    public async Task<ActionResult<bool>> UserExists(string email)
    {
        Console.WriteLine($"--> Checking if user {email} exists...");

        return await _authenticationService.UserAlreadyExistsAsync(email);
    }
}