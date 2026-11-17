using Microsoft.Extensions.DependencyInjection;
using MovieApp.Services.Abstractions;
using MovieApp.Services.Implementations;

namespace MovieApp.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDirectorService, DirectorService>();
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
