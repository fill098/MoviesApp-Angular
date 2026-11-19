using MovieApp.Dtos.Movies;

namespace MovieApp.Services.Abstractions;

public interface IMovieService
{
    Task<List<MovieReadDto>> GetAllMoviesAsync(CancellationToken cancellationToken);
    Task<MovieReadDto> GetMovieByIdAsync(int id, CancellationToken cancellationToken);
    Task<MovieReadDto> CreateMovieAsync(CreateMovieDto createNewMovie, CancellationToken cancellationToken);
    Task<MovieReadDto> UpdateMovieAsync(int id,  UpdateMovieDto updateMovie, CancellationToken cancellationToken = default);

    Task DeleteMovieAsync(int id, CancellationToken cancellationToken = default);
}
