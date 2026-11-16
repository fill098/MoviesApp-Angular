using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApp.Domain.Constants;

namespace MovieApp.DataAccess.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public const string AdminRoleId = "c7b013f0-5201-4317-abd8-c211f91b7330";
    public const string CustomerRoleId = "fab4fac1-c546-41de-aebc-a14da6895711";
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {

        builder.HasData(
            new IdentityRole
            {
                Id = AdminRoleId,
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpperInvariant(),
                ConcurrencyStamp = "a6f6b8a8-7f35-4d19-9d4b-6d5f5f0b9a01"
            },
            new IdentityRole
            {
                Id = CustomerRoleId,
                Name = Roles.Customer,
                NormalizedName = Roles.Customer.ToUpperInvariant(),
                ConcurrencyStamp = "0b5d8c34-3c2e-4a4c-8f0e-0d6c9a7e2b02"
            }
        );
    }
    
}




