namespace MovieApp.Services.Abstractions;

public interface ICurrentUser
{
    /// <summary>The logged-in user's id (the "sub" claim of the JWT).</summary>
    string Id { get; }
    bool IsAdmin { get; }
}
