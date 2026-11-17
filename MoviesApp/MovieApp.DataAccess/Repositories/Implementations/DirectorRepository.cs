using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Context;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Implementations;

public class DirectorRepository : Repository<Director>, IDirectorRepository
{
    public DirectorRepository(MoviesAppDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<List<Director>> GetAllAsync(CancellationToken cancellationToken)
    {
        IQueryable<Director> query = DbSet
            .AsNoTracking()
            .Include(d => d.Movies)          
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName);
        return query.ToListAsync(cancellationToken);
    }

    public override Task<Director?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Include(d => d.Movies)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public Task<Director?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(d => d.Movies)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}
