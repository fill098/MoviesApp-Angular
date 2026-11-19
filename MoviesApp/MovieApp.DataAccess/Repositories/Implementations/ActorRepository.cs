using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Context;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Implementations;

public class ActorRepository : Repository<Actor>, IActorRepository
{
    public ActorRepository(MoviesAppDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<List<Actor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<Actor> query = DbSet
            .AsNoTracking()
            .Include(a => a.Movies)
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName);
        return query.ToListAsync(cancellationToken);
    }

    public override async Task<Actor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(a => a.Movies)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }


    public async Task<Actor?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(a => a.Movies)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
    public Task<List<Actor>> GetByIdsTrackedAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken);
    }
}
