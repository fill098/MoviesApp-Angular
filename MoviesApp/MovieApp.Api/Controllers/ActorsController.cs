using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Domain.Constants;
using MovieApp.Dtos.Actors;
using MovieApp.Services.Abstractions;

namespace MovieApp.Api.Controllers;

public class ActorsController : ApiControllerBase
{
    private readonly IActorService _actorService;

    public ActorsController(IActorService actorService)
    {
        _actorService = actorService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorReadDto>>> GetAll(CancellationToken cancellation = default)
    {
        var actors = await _actorService.GetAllActorsAsync(cancellation);
        return Ok(actors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ActorReadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var actor = await _actorService.GetActorByIdAsync(id, cancellationToken);
        return Ok(actor);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ActorReadDto>> Create(CreateActorDto request, CancellationToken cancellationToken = default)
    {
        var newActor = await _actorService.CreateActorAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newActor.Id}, newActor);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ActorReadDto>> Update(int id, UpdateActorDto request, CancellationToken cancellationToken = default)
    {
        var updatedActor = await _actorService.UpdateActorAsync(id, request, cancellationToken);
        return Ok(updatedActor);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _actorService.DeleteActorAsync(id, cancellationToken);
        return NoContent();
    }
}
