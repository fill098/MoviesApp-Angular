using MapsterMapper;
using MovieApp.Common.Extensinons;
using MovieApp.DataAccess.Repositories.Abstractions;
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
        var validGenre = ParseGenre(createNewMovie.Genre);

        await EnsureDirectorExistsAsync(createNewMovie.DirectorId, cancellationToken);

        var validActors = await LoadActorsAsync(createNewMovie.ActorIds, cancellationToken);

        var movie = _mapper.Map<Movie>(createNewMovie);
        movie.Genre = validGenre;
        movie.Actors = validActors;
        movie.UserId = _currentUser.Id;

        await _movieRepository.AddAsync(movie, cancellationToken);

        // Reload so Director, Actors and Reviews are included for DirectorName/Actors/AverageRating
        var created = await _movieRepository.GetByIdAsync(movie.Id, cancellationToken);
        return _mapper.Map<MovieReadDto>(created!);
    }

    public async Task<MovieReadDto> UpdateMovieAsync(int id, UpdateMovieDto updateMovie, CancellationToken cancellationToken = default)
    {
        var movieDb = await _movieRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (movieDb is null)
        {
            throw new NotFoundException(nameof(Movie), id);
        }

        var validGenre = ParseGenre(updateMovie.Genre);
        await EnsureDirectorExistsAsync(updateMovie.DirectorId, cancellationToken);
        var validActors = await LoadActorsAsync(updateMovie.ActorIds, cancellationToken);

        _mapper.Map(updateMovie, movieDb);
        movieDb.Genre = validGenre;

        movieDb.Actors.Clear();
        foreach (var actor in validActors)
        {
            movieDb.Actors.Add(actor);
        }

        await _movieRepository.UpdateAsync(movieDb, cancellationToken);

        var updated = await _movieRepository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<MovieReadDto>(updated!);
    }
    public async Task DeleteMovieAsync(int id, CancellationToken cancellationToken = default)
    {
        var movieDb = await _movieRepository.GetByIdTrackedAsync(id, cancellationToken);
        if(movieDb is null)
        {
            throw new NotFoundException(nameof(Movie), id);
        }
        await _movieRepository.DeleteAsync(movieDb, cancellationToken);
    }

    #region Private helpers
    private static Genre ParseGenre(string value)
    {
        if (!Enum.TryParse<Genre>(value, ignoreCase: true, out var genre) || !Enum.IsDefined(genre))
        {
            throw new BadRequestException($"Invalid genre: {value}",
                [$"Allowed values: {string.Join(", ", Enum.GetNames<Genre>())}"]);
        }
        return genre;
    }

    private async Task EnsureDirectorExistsAsync(int directorId, CancellationToken cancellationToken)
    {
        if (!await _directorRepository.ExistsDirectorAsync(directorId, cancellationToken))
        {
            throw new BadRequestException($"Unknown director id: {directorId}");
        }
    }

    private async Task<List<Actor>> LoadActorsAsync(List<int> ids, CancellationToken cancellationToken)
    {
        var actorIds = ids.Distinct().ToList();
        var actors = await _actorRepository.GetByIdsTrackedAsync(actorIds, cancellationToken);
        if (actors.Count != actorIds.Count)
        {
            var missing = actorIds.Except(actors.Select(a => a.Id));
            throw new BadRequestException($"Unknown actor id(s): {string.Join(", ", missing)}");
        }
        return actors;
    }

    #endregion
}
