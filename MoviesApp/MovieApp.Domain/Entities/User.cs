using Microsoft.AspNetCore.Identity;

namespace MovieApp.Domain.Entities;

public class User : IdentityUser
{
    public List<Movie> Movies { get; set; } = [];
    public List<Director> Directors { get; set; } = [];
    public List<Actor> Actors { get; set; } = [];
    public List<Review> Reviews { get; set; } = [];
}
