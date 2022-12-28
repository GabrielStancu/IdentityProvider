using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Dtos;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
    ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number and 1 non alpha-numeric and at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string UserRole { get; set; } = string.Empty;
}