using MovieApp.Domain.Entities;

namespace MovieApp.Dtos.Actors;

public class ActorReadDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Biography { get; set; }
    public string? PhotoUrl { get; set; }

    public List<string> MoviesActedIn { get; set; } = new List<string>();

}
