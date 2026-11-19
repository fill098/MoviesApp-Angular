using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Context;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Implementations;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    public MovieRepository(MoviesAppDbContext dbContext) : base(dbContext)
    {
      
    }

    public override Task<List<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<Movie> query = DbSet
            .AsNoTracking()
            .Include(m => m.Director)
            .Include(m => m.Actors)
            .Include(m => m.Reviews)
            .OrderBy(m => m.Title);
        return query.ToListAsync(cancellationToken);
    }

    public override Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Include(m => m.Director)
            .Include(m => m.Actors)
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public Task<Movie?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(m => m.Director)
            .Include(m => m.Actors)
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }
}
