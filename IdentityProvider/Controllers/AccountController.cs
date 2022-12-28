using IdentityProvider.Dtos;
using IdentityProvider.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IRegisterService _registerService;

    public AccountController(ILoginService loginService, IRegisterService registerService)
    {
        _loginService = loginService;
        _registerService = registerService;
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _loginService.LoginAsync(loginDto);

        return user ?? (ActionResult<UserDto>)Unauthorized();
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var registerResult = await _registerService.RegisterAsync(registerDto);

        return registerResult.User ?? (ActionResult<UserDto>)BadRequest(registerResult.Message);
    }
}