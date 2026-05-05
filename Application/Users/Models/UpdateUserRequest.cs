namespace Application.Users.Models;

public class UpdateUserRequest
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Role { get; set; } = string.Empty;
}
