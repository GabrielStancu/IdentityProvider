using IdentityProvider.Dtos;
using IdentityProvider.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private const string _notFoundMessage = "Role not found!";

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> AllRoles()
    {
        var roles = await _roleService.AllRoles();

        return await Task.FromResult(Ok(roles));
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult<RoleDto>> RoleById(string id)
    {
        var role = await _roleService.RoleById(id);

        return role is null ? NotFound(_notFoundMessage) : Ok(role);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<RoleDto>> RoleByName(string name)
    {
        var role = await _roleService.RoleByName(name);

        return role is null ? NotFound(_notFoundMessage) : Ok(role);
    }

    [HttpPost("register/{roleName}")]
    public async Task<ActionResult<RoleDto>> Register(string roleName)
    {
        var role = await _roleService.Register(roleName);

        if (role is null)
            return BadRequest($"Failed to create the role {roleName}");

        return Ok(role);
    }

    [HttpPost("{id}")]
    public async Task<ActionResult> DeleteRole(string id)
    {
        bool deleted = await _roleService.DeleteRole(id);

        return deleted ? Ok() : NotFound(_notFoundMessage);
    }
}