namespace MovieApp.Dtos.Directors;

public class DirectorReadDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> MoviesDirected { get; set; } = [];
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Biography { get; set; }
    public string? PhotoUrl { get; set; }
}
