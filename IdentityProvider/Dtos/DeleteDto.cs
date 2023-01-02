using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Dtos;

public class DeleteDto
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}