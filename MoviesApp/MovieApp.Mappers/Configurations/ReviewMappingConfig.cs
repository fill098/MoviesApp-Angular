using Mapster;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Reviews;

namespace MovieApp.Mappers.Configurations;

public class ReviewMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Review, ReviewReadDto>()
        .Map(dest => dest.MovieTitle, src => src.Movie.Title)
        .Map(dest => dest.UserName, src => src.User!.UserName);

    }
}
