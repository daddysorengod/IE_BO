namespace Application.Security;

public interface ISuperUserSessionStore
{
    void Save(string refreshToken, DateTime refreshTokenExpiresAtUtc);
    bool HasValidSession();
    bool IsValidRefreshToken(string refreshToken);
    void Clear();
}
