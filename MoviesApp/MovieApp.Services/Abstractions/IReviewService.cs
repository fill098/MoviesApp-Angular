using MovieApp.Dtos.Reviews;

namespace MovieApp.Services.Abstractions;

public interface IReviewService
{
    Task<List<ReviewReadDto>> GetAllReviewsAsync(CancellationToken cancellationToken);
    Task<ReviewReadDto> GetReviewByIdAsync(int id, CancellationToken cancellationToken);
    Task<ReviewReadDto> CreateReviewAsync(CreateReviewDto createReview, CancellationToken cancellationToken);
    Task<ReviewReadDto> UpdateReviewAsync(int id, UpdateReviewDto updateReview, CancellationToken cancellationToken);
}
