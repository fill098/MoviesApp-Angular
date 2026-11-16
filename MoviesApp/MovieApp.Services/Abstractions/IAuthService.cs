using MovieApp.Dtos.Auth;
using MovieApp.Dtos.Users;

namespace MovieApp.Services.Abstractions;

public interface IAuthService
{
    Task<UserReadDto> RegisterAsync(RegisterRequestUserDto request);
    Task<LogInUserResponseDto> LogInAsync(LogInRequestUserDto request);
}
