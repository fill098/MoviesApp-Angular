using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoviesApp.DataAccess.Context;
using MoviesApp.Domain.Entities;

namespace MoviesApp.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        // Stop at startup with a clear message instead of failing on the first database call.
        var connectionString = configuration.GetConnectionString("MoviesAppDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'MoviesAppDb' is missing. Set it in appsettings.Development.json.");
        }

        // Database
        services.AddDbContext<MoviesAppDbContext>(options => options.UseNpgsql(connectionString));

        // Identity: UserManager<User> and RoleManager<IdentityRole>, stored through our DbContext
        services.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MoviesAppDbContext>();

        return services;
    }
}
