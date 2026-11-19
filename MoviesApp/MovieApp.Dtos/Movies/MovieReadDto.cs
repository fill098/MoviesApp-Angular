namespace MovieApp.Dtos.Movies;

public class MovieReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int DurationMinutes { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }

    public int DirectorId { get; set; }
    public string DirectorName { get; set; } = string.Empty;

    public List<string> Actors { get; set; } = new List<string>();

    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}


