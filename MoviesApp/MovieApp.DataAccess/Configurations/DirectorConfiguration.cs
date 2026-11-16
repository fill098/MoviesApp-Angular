using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Configurations;

public class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.ToTable("Directors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Nationality)
            .HasMaxLength(100);

        builder.Property(d => d.Biography)
            .HasMaxLength(2000);

        builder.Property(d => d.PhotoUrl)
            .HasMaxLength(500);

        // Built from FirstName + LastName in C#, not a database column.
        builder.Ignore(d => d.FullName);

        // Speeds up searching and sorting directors by name.
        builder.HasIndex(d => new { d.LastName, d.FirstName });

        // The user who added the director (optional).
        // SetNull: deleting that user keeps the director and just clears UserId.
        builder.HasOne(d => d.User)
            .WithMany(u => u.Directors)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // The Director -> Movies relationship (one director, many movies)
        // is configured once, in MovieConfiguration.
    }
}
