using MapsterMapper;
using MovieApp.Common.Extensinons;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Directors;
using MovieApp.Services.Abstractions;

namespace MovieApp.Services.Implementations;

public class DirectorService : IDirectorService
{
    private readonly IDirectorRepository _directorRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public DirectorService(IDirectorRepository directorRepository, IMapper mapper, ICurrentUser currentUser)
    {
        _directorRepository = directorRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }


    public async Task<List<DirectorReadDto>> GetAllDirectorsAsync(CancellationToken cancellationToken)
    {

        List<Director> directors = await _directorRepository.GetAllAsync(cancellationToken);

        List<DirectorReadDto> directorDtos = _mapper.Map<List<DirectorReadDto>>(directors);

        return directorDtos;
    }

    public async Task<DirectorReadDto> GetDirectorByIdAsync(int id, CancellationToken cancellationToken)
    {
        var directorDb = await _directorRepository.GetByIdAsync(id, cancellationToken);
        if (directorDb == null)
        {
            throw new NotFoundException(nameof(Director), id);
        }
        var directorDto = _mapper.Map<DirectorReadDto>(directorDb);
        return directorDto;
    }
    public async Task<DirectorReadDto> CreateDirectorAsync(CreateDirectorDto request, CancellationToken cancellationToken)
    {
        var director = _mapper.Map<Director>(request);

        director.UserId = _currentUser.Id;

        await _directorRepository.AddAsync(director, cancellationToken);

        return _mapper.Map<DirectorReadDto>(director);
    }

    public async Task<DirectorReadDto> UpdateDirectorAsync(int id, UpdateDirectorDto request, CancellationToken cancellationToken)
    {
        var directorDb = await _directorRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (directorDb == null)
        {
            throw new NotFoundException(nameof(Director), id);
        }
        _mapper.Map(request, directorDb);
        await _directorRepository.UpdateAsync(directorDb, cancellationToken);
        return _mapper.Map<DirectorReadDto>(directorDb);
    }

    public async Task DeleteDirectorAsync(int id, CancellationToken cancellationToken)
    {
        var directorDb = await _directorRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (directorDb == null)
        {
            throw new NotFoundException(nameof(Director), id);
        }
        if (directorDb.Movies.Count > 0)
        {
            throw new ConflictException(
                $"{directorDb.FullName} can't be deleted because they still have {directorDb.Movies.Count} movie(s). Delete or reassign those movies first.");
        }
        await _directorRepository.DeleteAsync(directorDb, cancellationToken);
    }
}


