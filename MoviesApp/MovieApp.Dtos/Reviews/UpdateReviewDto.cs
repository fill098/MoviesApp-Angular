using System.ComponentModel.DataAnnotations;

namespace MovieApp.Dtos.Reviews;

public class UpdateReviewDto
{
    [Range(1, 10)]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}
