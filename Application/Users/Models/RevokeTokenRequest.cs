namespace Application.Users.Models;

public class RevokeTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
