namespace MovieApp.Dtos.Auth;

public class LogInUserResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}
