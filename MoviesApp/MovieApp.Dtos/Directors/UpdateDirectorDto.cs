using System.ComponentModel.DataAnnotations;

namespace MovieApp.Dtos.Directors;

public class UpdateDirectorDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(100)]
    public string? Nationality { get; set; }

    [StringLength(2000)]
    public string? Biography { get; set; }

    [Url]
    [StringLength(500)]
    public string? PhotoUrl { get; set; }
}
