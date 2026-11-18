using MapsterMapper;
using MovieApp.Common.Extensinons;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Actors;
using MovieApp.Services.Abstractions;

namespace MovieApp.Services.Implementations;

public class ActorService : IActorService
{
    private readonly IActorRepository _actorRepository;
    private readonly IMapper _mapper;

    private readonly ICurrentUser _currentUser;

    public ActorService(IActorRepository actorRepository, IMapper mapper, ICurrentUser currentUser)
    {
        _actorRepository = actorRepository;
        _mapper = mapper;
        _currentUser = currentUser;

    }


    public async Task<List<ActorReadDto>> GetAllActorsAsync(CancellationToken cancellation = default)
    {
        List<Actor> actorsDb = await _actorRepository.GetAllAsync(cancellation);

        List<ActorReadDto> actorReadDtos = _mapper.Map<List<ActorReadDto>>(actorsDb);

        return actorReadDtos;
    }
    public async Task<ActorReadDto> GetActorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        Actor? actor = await _actorRepository.GetByIdAsync(id, cancellationToken);
        if(actor is null)
        {
            throw new NotFoundException(nameof(Actor), id);
        }

        return _mapper.Map<ActorReadDto>(actor);
    }

    public async Task<ActorReadDto> CreateActorAsync(CreateActorDto request, CancellationToken cancellationToken = default)
    {
        var newActor = _mapper.Map<Actor>(request);

        newActor.UserId = _currentUser.Id;

        await _actorRepository.AddAsync(newActor, cancellationToken);

        return _mapper.Map<ActorReadDto>(newActor);
    }
}
