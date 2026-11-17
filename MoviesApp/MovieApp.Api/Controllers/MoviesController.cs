using Microsoft.AspNetCore.Mvc;
using MovieApp.Dtos.Movies;
using MovieApp.Services.Abstractions;

namespace MovieApp.Api.Controllers;

public class MoviesController : ApiControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]

    public async Task<ActionResult<List<MovieReadDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        var movies = await _movieService.GetAllMoviesAsync(cancellationToken);
        return Ok(movies);
    }
}
