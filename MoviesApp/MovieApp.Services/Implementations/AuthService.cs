using Microsoft.AspNetCore.Identity;
using MovieApp.Common.Extensinons;
using MovieApp.Dtos.Auth;
using MovieApp.Dtos.Users;
using MovieApp.Services.Abstractions;
using MovieApp.Services.Models;
using MovieApp.Domain.Constants;
using MovieApp.Domain.Entities;

namespace MovieApp.Services.Implementations;

public class AuthService : IAuthService
{

    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(ITokenService tokenService, UserManager<User> userManager) 
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }
    public async Task<UserReadDto> RegisterAsync(RegisterRequestUserDto request)
    {
        User newUser = new User 
        {
            UserName = request.UserName,
            Email = request.Email,
        };

        IdentityResult createNewUser = await _userManager.CreateAsync(newUser, request.Password);

        if (!createNewUser.Succeeded)
        {
            throw new BadRequestException("Registration failed.", createNewUser.Errors.Select(error => error.Description));
        }

        await _userManager.AddToRoleAsync(newUser, Roles.Customer);

        var userReadDto = new UserReadDto
        {
            Id = newUser.Id,
            Email = newUser.Email,
            UserName = newUser.UserName,
            Roles = [Roles.Customer]
        };

        return userReadDto;
    }

    public async Task<LogInUserResponseDto> LogInAsync(LogInRequestUserDto request)
    {
        User? user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        TokenResult tokenResult = _tokenService.CreateToken(user, roles);

        return new LogInUserResponseDto
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            UserName = user.UserName ?? string.Empty,
            Roles = [.. roles]
        };

    }
}
