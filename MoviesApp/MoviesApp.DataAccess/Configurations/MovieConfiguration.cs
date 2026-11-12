using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviesApp.Domain.Entities;

namespace MoviesApp.DataAccess.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies", t =>
            t.HasCheckConstraint("CK_Movies_DurationMinutes", "\"DurationMinutes\" > 0"));

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.Property(m => m.ReleaseDate)
            .IsRequired();

        builder.Property(m => m.DurationMinutes)
            .IsRequired();

        // Store the enum as text ("Action", "Drama"...) so the database stays readable
        // and reordering the enum can never corrupt existing rows.
        builder.Property(m => m.Genre)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.PosterUrl)
            .HasMaxLength(500);

        builder.Property(m => m.TrailerUrl)
            .HasMaxLength(500);

        // Calculated from Reviews in memory, not a database column.
        builder.Ignore(m => m.AverageRating);

        builder.HasIndex(m => m.Title);
        builder.HasIndex(m => m.Genre);

        // Many movies -> one director. Restrict: a director who still has movies can't be deleted.
        builder.HasOne(m => m.Director)
            .WithMany(d => d.Movies)
            .HasForeignKey(m => m.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // The user who added the movie (optional).
        // SetNull: deleting that user keeps the movie and just clears UserId.
        builder.HasOne(m => m.User)
            .WithMany(u => u.Movies)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // One movie -> many reviews. Deleting a movie deletes its reviews.
        builder.HasMany(m => m.Reviews)
            .WithOne(r => r.Movie)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many movies <-> many actors through a "MovieActors" join table.
        builder.HasMany(m => m.Actors)
            .WithMany(a => a.Movies)
            .UsingEntity<Dictionary<string, object>>(
                "MovieActors",
                right => right.HasOne<Actor>().WithMany().HasForeignKey("ActorId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Movie>().WithMany().HasForeignKey("MovieId").OnDelete(DeleteBehavior.Cascade),
                join => join.HasKey("MovieId", "ActorId"));
    }
}
