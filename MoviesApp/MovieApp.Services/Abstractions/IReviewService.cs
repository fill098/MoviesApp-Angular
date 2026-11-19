using MovieApp.Dtos.Reviews;

namespace MovieApp.Services.Abstractions;

public interface IReviewService
{
    Task<List<ReviewReadDto>> GetAllReviewsAsync(CancellationToken cancellationToken);

    Task<ReviewReadDto> GetReviewByIdAsync(int id, CancellationToken cancellationToken);
}
