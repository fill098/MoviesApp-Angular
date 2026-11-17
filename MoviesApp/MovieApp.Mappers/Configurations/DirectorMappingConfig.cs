using Mapster;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Directors;

namespace MovieApp.Mappers.Configurations;

public class DirectorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Director, DirectorReadDto>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.MoviesDirected, src => src.Movies.Select(m => m.Title).ToList());
        config.NewConfig<CreateDirectorDto, Director>();
        config.NewConfig<UpdateDirectorDto, Director>();

    }
}
