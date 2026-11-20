using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Context;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Repositories.Implementations;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(MoviesAppDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<List<Review>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().OrderByDescending(r => r.CreatedDate);
        return SelectForRead(query).ToListAsync(cancellationToken);
    }

    public override Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(r => r.Id == id);
        return SelectForRead(query).FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<Review> SelectForRead(IQueryable<Review> query)
    {
        return query.Select(r => new Review
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            MovieId = r.MovieId,
            UserId = r.UserId,
            CreatedDate = r.CreatedDate,
            UpdatedDate = r.UpdatedDate,
            Movie = new Movie { Title = r.Movie.Title },
            User = new User { UserName = r.User!.UserName }
        });
    }

    public Task<bool> ExistsForUserAndMovieAsync(string userId, int movieId, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(r => r.UserId == userId && r.MovieId == movieId, cancellationToken);
    }

    public Task<Review?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
