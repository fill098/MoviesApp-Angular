using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Domain.Constants;
using MovieApp.Dtos.Directors;
using MovieApp.Services.Abstractions;


namespace MovieApp.Api.Controllers;


public class DirectorsController : ApiControllerBase
{

    private readonly IDirectorService _directorService;



    public DirectorsController(IDirectorService directorService)
    {
        _directorService = directorService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DirectorReadDto>>> GetAll(CancellationToken cancellationToken = default)
    {
       var directors = await _directorService.GetAllDirectorsAsync(cancellationToken);
        return Ok(directors);
    }

    [HttpGet("{id:int}")]

    public async Task<ActionResult<DirectorReadDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var director = await _directorService.GetDirectorByIdAsync(id, cancellationToken);
        return Ok(director);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<DirectorReadDto>> Create(CreateDirectorDto request, CancellationToken cancellationToken = default)
    {
        var director = await _directorService.CreateDirectorAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = director.Id }, director);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<DirectorReadDto>> Update(int id, UpdateDirectorDto request, CancellationToken cancellationToken = default)
    {
        var director = await _directorService.UpdateDirectorAsync(id, request, cancellationToken);
        return Ok(director);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _directorService.DeleteDirectorAsync(id, cancellationToken);
        return NoContent();
    }
}
