using MapsterMapper;
using MovieApp.Common.Extensinons;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.DataAccess.Repositories.Implementations;
using MovieApp.Domain.Entities;
using MovieApp.Domain.Enums;
using MovieApp.Dtos.Movies;
using MovieApp.Services.Abstractions;

namespace MovieApp.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    private readonly IDirectorRepository _directorRepository;

    private readonly IActorRepository _actorRepository;
    public MovieService(IMovieRepository movieRepository, IMapper mapper, ICurrentUser currentUser, IDirectorRepository directorRepository, IActorRepository actorRepository)
    {
        _movieRepository = movieRepository;
        _mapper = mapper;
        _currentUser = currentUser;
        _directorRepository = directorRepository;
        _actorRepository = actorRepository;
    }

    public async Task<List<MovieReadDto>> GetAllMoviesAsync(CancellationToken cancellationToken = default)
    {
        List<Movie> moviesDb = await _movieRepository.GetAllAsync(cancellationToken);

        List<MovieReadDto> movieReadDtos = _mapper.Map<List<MovieReadDto>>(moviesDb);

        return movieReadDtos;
    }

    public async Task<MovieReadDto> GetMovieByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var movieDb = await _movieRepository.GetByIdAsync(id, cancellationToken);
        if (movieDb == null)
        {
            throw new NotFoundException(nameof(Movie), id);
        }
        var movieDto = _mapper.Map<MovieReadDto>(movieDb);
        return movieDto;
    }

    public async Task<MovieReadDto> CreateMovieAsync(CreateMovieDto createNewMovie, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<Genre>(createNewMovie.Genre, ignoreCase: true, out var genre) || !Enum.IsDefined(genre))
        {
            throw new BadRequestException($"Invalid genre: {createNewMovie.Genre}",
                [$"Allowed values: {string.Join(", ", Enum.GetNames<Genre>())}"]);
        }

        if (!await _directorRepository.ExistsDirectorAsync(createNewMovie.DirectorId, cancellationToken))
        {
            throw new NotFoundException(nameof(Director), createNewMovie.DirectorId);
        }

        var actorIds = createNewMovie.ActorIds.Distinct().ToList();
        var actors = await _actorRepository.GetByIdsTrackedAsync(actorIds, cancellationToken);
        if (actors.Count != actorIds.Count)
        {
            var missing = actorIds.Except(actors.Select(a => a.Id));
            throw new BadRequestException($"Unknown actor id(s): {string.Join(", ", missing)}");
        }

        var movie = _mapper.Map<Movie>(createNewMovie);
        movie.Genre = genre;
        movie.Actors = actors;
        movie.UserId = _currentUser.Id;

        await _movieRepository.AddAsync(movie, cancellationToken);

        // Reload so Director, Actors and Reviews are included for DirectorName/Actors/AverageRating
        var created = await _movieRepository.GetByIdAsync(movie.Id, cancellationToken);
        return _mapper.Map<MovieReadDto>(created!);
    }

}
