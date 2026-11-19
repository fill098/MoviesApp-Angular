using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Domain.Constants;
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieReadDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var movie = await _movieService.GetMovieByIdAsync(id, cancellationToken);
        return Ok(movie);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]

    public async Task<ActionResult<MovieReadDto>> Create(CreateMovieDto request, CancellationToken cancellationToken = default)
    {
        var movie = await _movieService.CreateMovieAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = Roles.Admin)]

    public async Task<ActionResult<MovieReadDto>> Update(int id, UpdateMovieDto request, CancellationToken cancellationToken = default)
    {
        var movie = await _movieService.UpdateMovieAsync(id, request, cancellationToken);
        return Ok(movie);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _movieService.DeleteMovieAsync(id, cancellationToken);
        return NoContent();
    }
}
