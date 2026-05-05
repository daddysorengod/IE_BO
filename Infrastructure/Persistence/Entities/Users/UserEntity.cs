namespace Infrastructure.Persistence.Entities.Users;

internal class UserEntity
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } 
    public string? Delt { get; set; }
    public string? IsActive { get; set; }
}
