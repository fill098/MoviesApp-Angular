using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Abstractions;

public interface IMovieRepository : IRepository<Movie>
{
    Task<Movie?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default);
}
