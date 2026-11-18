using MapsterMapper;
using MovieApp.Common.Extensinons;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Movies;
using MovieApp.Services.Abstractions;

namespace MovieApp.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    //private readonly IActorRepository _actorRepository;
    public MovieService(IMovieRepository movieRepository, IMapper mapper, ICurrentUser currentUser)
    {
        _movieRepository = movieRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<MovieReadDto>> GetAllMoviesAsync(CancellationToken cancellationToken)
    {
        List<Movie> moviesDb = await _movieRepository.GetAllAsync(cancellationToken);

        List<MovieReadDto> movieReadDtos = _mapper.Map<List<MovieReadDto>>(moviesDb);

        return movieReadDtos;
    }

    public async Task<MovieReadDto> GetMovieByIdAsync(int id, CancellationToken cancellationToken)
    {
        var movieDb = await _movieRepository.GetByIdAsync(id, cancellationToken);
        if (movieDb == null)
        {
            throw new NotFoundException(nameof(Movie), id);
        }
        var movieDto = _mapper.Map<MovieReadDto>(movieDb);
        return movieDto;
    }
}
