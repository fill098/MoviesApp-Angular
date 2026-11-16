using MovieApp.Domain.Common;

namespace MovieApp.Domain.Entities;

public class Review : BaseEntity
{
    public const int MinRating = 1;
    public const int MaxRating = 10;

    public int Rating { get; set; }
    public string? Comment { get; set; }

    // Identity user id of the customer who wrote the review
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
}
