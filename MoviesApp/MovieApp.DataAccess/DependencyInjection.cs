using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataAccess.Context;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.DataAccess.Repositories.Implementations;
using MovieApp.DataAccess.Seed;
using MovieApp.Domain.Entities;

namespace MovieApp.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        // Stop at startup with a clear message instead of failing on the first database call.
        var connectionString = configuration.GetConnectionString("MovieAppDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'MovieAppDb' is missing. Set it in the API project's user secrets.");
        }

        // Database. UseSeeding/UseAsyncSeeding run TestDataSeeder on Update-Database
        // ("dotnet ef database update") and Database.Migrate(); it only adds data to an empty database.
        services.AddDbContext<MoviesAppDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSeeding((context, _) => TestDataSeeder.Seed(context))
            .UseAsyncSeeding((context, _, cancellationToken) => TestDataSeeder.SeedAsync(context, cancellationToken)));

        // Identity: UserManager<User> and RoleManager<IdentityRole>, stored through our DbContext
        services.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MoviesAppDbContext>();

        services.AddScoped<IDirectorRepository, DirectorRepository>();
        services.AddScoped<IMovieRepository, MovieRepository>();


        return services;
    }
}
