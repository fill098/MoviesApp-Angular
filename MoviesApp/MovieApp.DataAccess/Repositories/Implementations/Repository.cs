using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.DataAccess.Context;
using MovieApp.Domain.Common;

namespace MovieApp.DataAccess.Repositories.Implementations;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected MoviesAppDbContext DbContext { get; init; }

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    protected Repository(MoviesAppDbContext dbContext)
    {
        DbContext = dbContext;
    }
    public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (DbContext.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Update(entity);
        }
        await DbContext.SaveChangesAsync(cancellationToken);
    }
    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
