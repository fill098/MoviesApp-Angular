using Mapster;
using MovieApp.Dtos.Users;
using MovieApp.Domain.Entities;

namespace MovieApp.Mappers.Configurations;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserReadDto>()
            .Ignore(dest => dest.Roles);
    }
}
