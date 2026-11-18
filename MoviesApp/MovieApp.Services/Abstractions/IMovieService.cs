using MovieApp.Dtos.Movies;

namespace MovieApp.Services.Abstractions;

public interface IMovieService
{
    Task<List<MovieReadDto>> GetAllMoviesAsync(CancellationToken cancellationToken);
    Task<MovieReadDto> GetMovieByIdAsync(int id, CancellationToken cancellationToken);
}
