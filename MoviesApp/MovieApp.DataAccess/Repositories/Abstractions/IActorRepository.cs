using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Abstractions;

public interface IActorRepository : IRepository<Actor>
{
    Task<Actor?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Actor>> GetByIdsTrackedAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
