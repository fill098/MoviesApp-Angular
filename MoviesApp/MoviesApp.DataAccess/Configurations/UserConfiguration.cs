using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviesApp.Domain.Entities;

namespace MoviesApp.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public const string AdminUserId = "860135b1-1222-4cd1-9dff-c04fb8c94843";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Seed Admin user
        builder.HasData(new User
        {
            Id = AdminUserId,

            // Identity finds users by the normalized (upper-case) name and email
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@moviesapp.local",
            NormalizedEmail = "ADMIN@MOVIESAPP.LOCAL",
            EmailConfirmed = true,

            // Admin123!
            PasswordHash = "AQAAAAIAAYagAAAAEORMNJTSPwqrmb2GKtm5oxli8mkCddIShGuomTmTzpYUfNBuyB9YyZyhymbYQOXTMA==",

            SecurityStamp = "ea93155b-08c2-4539-a6d5-e922d27173e3",
            ConcurrencyStamp = "d476b4e7-13dc-42c4-a6a2-0c1c5e8897be"
        });
    }
}
