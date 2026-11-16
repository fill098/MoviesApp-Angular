using MovieApp.Domain.Common;

namespace MovieApp.Domain.Entities;

public class Actor : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Biography { get; set; }
    public string? PhotoUrl { get; set; }

    public string? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();

    public string FullName => $"{FirstName} {LastName}";
}
