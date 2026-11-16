using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Dtos.Auth;

public class RegisterRequestUserDto
{
    [Required]
    [StringLength(50), MinLength(3)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength (256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;

}
