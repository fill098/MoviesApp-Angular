namespace MovieApp.Dtos.Reviews;

public class ReviewReadDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; } 
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
