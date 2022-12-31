namespace IdentityProvider.Dtos;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public RoleDto Role { get; set; } = null!;
}