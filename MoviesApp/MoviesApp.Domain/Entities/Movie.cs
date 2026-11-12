using MoviesApp.Domain.Common;
using MoviesApp.Domain.Enums;

namespace MoviesApp.Domain.Entities;

public class Movie : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int DurationMinutes { get; set; }
    public Genre Genre { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }

    public string? UserId { get; set; }
    public User? User { get; set; }

    public int DirectorId { get; set; }
    public Director Director { get; set; } = null!;

    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public double AverageRating => Reviews.Count == 0 ? 0 : Reviews.Average(r => r.Rating);
}
