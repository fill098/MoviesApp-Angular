using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Abstractions;

public interface IDirectorRepository : IRepository<Director>
{
    Task<Director?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default);
}
