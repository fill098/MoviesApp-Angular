using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Dtos.Auth;
using MovieApp.Dtos.Users;
using MovieApp.Services.Abstractions;

namespace MovieApp.Api.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserReadDto>> Register(RegisterRequestUserDto request, CancellationToken cancellationToken)
    {
        var user = await _authService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LogInUserResponseDto>> LogIn(LogInRequestUserDto request, CancellationToken cancellationToken)
    {
        var response = await _authService.LogInAsync(request);
        return Ok(response);
    }


}
