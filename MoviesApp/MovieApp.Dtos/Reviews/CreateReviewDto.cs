using System.ComponentModel.DataAnnotations;

namespace MovieApp.Dtos.Reviews;

public class CreateReviewDto
{
    [Range(1, int.MaxValue)]
    public int MovieId { get; set; }
    [Range(1,10)]
    public int Rating { get; set; }
    [StringLength(2000)]
    public string? Comment { get; set; }
}
