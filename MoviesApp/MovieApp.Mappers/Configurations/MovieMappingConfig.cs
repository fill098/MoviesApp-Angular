using Mapster;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Movies;

namespace MovieApp.Mappers.Configurations;

public class MovieMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Movie, MovieReadDto>()
            .Map(dest => dest.DirectorName, src => src.Director.FullName)
            .Map(dest => dest.Actors, src => src.Actors.Select(a => a.FullName).ToList())
            .Map(dest => dest.AverageRating, src => src.Reviews.Count == 0 ? 0 : Math.Round(src.Reviews.Average(r => r.Rating), 1))
            .Map(dest => dest.ReviewCount, src => src.Reviews.Count);
        config.NewConfig<CreateMovieDto, Movie>()
            .Ignore(dest => dest.Genre);
    }
}
