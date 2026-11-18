using System.ComponentModel.DataAnnotations;

namespace MovieApp.Dtos.Movies;

public class CreateMovieDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DateOnly? ReleaseDate { get; set; }

    [Range(1, 1000)]
    public int DurationMinutes { get; set; }

    [Required]
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;

    [Url]
    [StringLength(500)]
    public string? PosterUrl { get; set; }

    [Url]
    [StringLength(500)]
    public string? TrailerUrl { get; set; }

    [Range(1, int.MaxValue)]
    public int DirectorId { get; set; }

    public List<int> ActorIds { get; set; } = [];
}
