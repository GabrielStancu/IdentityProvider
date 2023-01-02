using IdentityProvider.Dtos;
using IdentityProvider.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> AllUsersAsync()
    {
        Console.WriteLine("--> Getting all users...");

        var users = await _userService.GetUsersAsync();

        return Ok(users);
    }

    [HttpGet("{roleName}")]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> AllUsersInRoleAsync(string roleName)
    {
        Console.WriteLine($"--> Getting all users with role = {roleName}...");

        var users = await _userService.GetUsersInRoleAsync(roleName);

        return Ok(users);
    }

    [HttpPost("delete")]
    public async Task<ActionResult> Delete(DeleteDto deleteDto)
    {
        Console.WriteLine($"--> Deleting user {deleteDto.Email}...");

        var deleted = await _userService.DeleteUserAsync(deleteDto);

        return deleted ? Ok() : BadRequest();
    }

    [HttpPost("exists/{email}")]
    public async Task<ActionResult<bool>> UserExists(string email)
    {
        Console.WriteLine($"--> Checking if user {email} exists...");

        return await _userService.UserAlreadyExistsAsync(email);
    }
}