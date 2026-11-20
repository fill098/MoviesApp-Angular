using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Abstractions;

public interface IReviewRepository : IRepository<Review>
{
    Task<bool> ExistsForUserAndMovieAsync(string userId, int movieId, CancellationToken cancellationToken = default);
    Task<Review?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default);
}
