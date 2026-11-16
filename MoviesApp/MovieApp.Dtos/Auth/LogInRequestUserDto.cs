using System.ComponentModel.DataAnnotations;

namespace MovieApp.Dtos.Auth;

public class LogInRequestUserDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
