using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviesApp.Domain.Entities;

namespace MoviesApp.DataAccess.Configurations;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("Actors");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Nationality)
            .HasMaxLength(100);

        builder.Property(a => a.Biography)
            .HasMaxLength(2000);

        builder.Property(a => a.PhotoUrl)
            .HasMaxLength(500);

        // Built from FirstName + LastName in C#, not a database column.
        builder.Ignore(a => a.FullName);

        // Speeds up searching and sorting actors by name.
        builder.HasIndex(a => new { a.LastName, a.FirstName });

        // The user who added the actor (optional).
        // SetNull: deleting that user keeps the actor and just clears UserId.
        builder.HasOne(a => a.User)
            .WithMany(u => u.Actors)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // The Movies <-> Actors relationship (the "MovieActors" join table)
        // is configured once, in MovieConfiguration.
    }
}
