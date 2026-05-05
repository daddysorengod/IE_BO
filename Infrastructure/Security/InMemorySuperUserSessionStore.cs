using Application.Security;

namespace Infrastructure.Security;

public sealed class InMemorySuperUserSessionStore : ISuperUserSessionStore
{
    private readonly object _syncRoot = new();
    private string? _refreshToken;
    private DateTime? _refreshTokenExpiresAtUtc;

    public void Save(string refreshToken, DateTime refreshTokenExpiresAtUtc)
    {
        lock (_syncRoot)
        {
            _refreshToken = refreshToken;
            _refreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;
        }
    }

    public bool HasValidSession()
    {
        lock (_syncRoot)
        {
            if (string.IsNullOrWhiteSpace(_refreshToken)
                || _refreshTokenExpiresAtUtc is null
                || _refreshTokenExpiresAtUtc <= DateTime.UtcNow)
            {
                ClearUnsafe();
                return false;
            }

            return true;
        }
    }

    public bool IsValidRefreshToken(string refreshToken)
    {
        lock (_syncRoot)
        {
            if (string.IsNullOrWhiteSpace(_refreshToken)
                || _refreshTokenExpiresAtUtc is null
                || _refreshTokenExpiresAtUtc <= DateTime.UtcNow)
            {
                ClearUnsafe();
                return false;
            }

            return string.Equals(_refreshToken, refreshToken, StringComparison.Ordinal);
        }
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            ClearUnsafe();
        }
    }

    private void ClearUnsafe()
    {
        _refreshToken = null;
        _refreshTokenExpiresAtUtc = null;
    }
}
