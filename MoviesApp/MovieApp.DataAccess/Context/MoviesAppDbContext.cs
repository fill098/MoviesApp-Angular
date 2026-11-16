using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApp.Domain.Common;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Context;

// IdentityDbContext adds the ASP.NET Identity tables (AspNetUsers, AspNetRoles, ...)
// next to our own tables, so users, roles and movies live in one database.
public class MoviesAppDbContext : IdentityDbContext<User>
{
    public MoviesAppDbContext(DbContextOptions<MoviesAppDbContext> options) :base(options)
    {
        
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Director> Directors => Set<Director>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Must run first: it configures the Identity tables.
        base.OnModelCreating(builder);

        // Picks up every IEntityTypeConfiguration in this project
        // (MovieConfiguration, ActorConfiguration, DirectorConfiguration, ReviewConfiguration).
        builder.ApplyConfigurationsFromAssembly(typeof(MoviesAppDbContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetUpdatedDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        SetUpdatedDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    // Keeps UpdatedDate correct automatically whenever a row is edited.
    private void SetUpdatedDates()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDate = DateTime.UtcNow;
            }
        }
    }
}
