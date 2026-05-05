namespace Application.Users.Models;

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Role { get; set; } = string.Empty;
}
