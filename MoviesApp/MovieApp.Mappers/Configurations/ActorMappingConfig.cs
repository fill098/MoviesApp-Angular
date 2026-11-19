using Mapster;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Actors;

namespace MovieApp.Mappers.Configurations;

public class ActorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Actor, ActorReadDto>()
            .Map(dest => dest.MoviesActedIn, src => src.Movies.Select(m => m.Title).ToList());
        config.NewConfig<CreateActorDto, Actor>();
        config.NewConfig<UpdateActorDto, Actor>();
    }
}
