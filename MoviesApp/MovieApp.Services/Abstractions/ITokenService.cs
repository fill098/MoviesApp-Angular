using MovieApp.Services.Models;
using MovieApp.Domain.Entities;

namespace MovieApp.Services.Abstractions;

public interface ITokenService
{
    TokenResult CreateToken(User user, IEnumerable<string> roles);
}
