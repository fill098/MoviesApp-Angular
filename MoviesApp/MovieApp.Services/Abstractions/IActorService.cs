using MovieApp.Dtos.Actors;

namespace MovieApp.Services.Abstractions;

public interface IActorService
{
    Task<List<ActorReadDto>> GetAllActorsAsync(CancellationToken cancellation = default);
    Task<ActorReadDto> GetActorByIdAsync(int  id, CancellationToken cancellationToken = default);
    Task<ActorReadDto> CreateActorAsync(CreateActorDto request, CancellationToken cancellationToken = default);
    Task<ActorReadDto> UpdateActorAsync(int id, UpdateActorDto request, CancellationToken cancellationToken = default);
    Task DeleteActorAsync(int id, CancellationToken cancellationToken = default);
}
