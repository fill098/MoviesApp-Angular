using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // The database itself rejects ratings outside Review.MinRating..Review.MaxRating (1-10).
        builder.ToTable("Reviews", t =>
            t.HasCheckConstraint(
                "CK_Reviews_Rating",
                $"\"Rating\" BETWEEN {Review.MinRating} AND {Review.MaxRating}"));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(2000);

        // Every review must have an author. No max length: it has to match
        // AspNetUsers.Id exactly, which Identity stores as text on PostgreSQL.
        builder.Property(r => r.UserId)
            .IsRequired();

        // The user who wrote the review (required).
        // Cascade: deleting a user deletes their reviews.
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // A user can review each movie only once.
        // This index also makes "all reviews by this user" fast.
        builder.HasIndex(r => new { r.UserId, r.MovieId })
            .IsUnique();

        // The Movie -> Reviews relationship (one movie, many reviews, cascade delete)
        // is configured once, in MovieConfiguration.
    }
}
