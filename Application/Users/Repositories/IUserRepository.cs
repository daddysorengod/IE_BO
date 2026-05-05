using Domain.Users;

namespace Application.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<long> InsertAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long userId, CancellationToken cancellationToken = default);
    Task<bool> ActivateAsync(long userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateRefreshTokenAsync(long userId, string? refreshToken, DateTime? refreshTokenExpiresAtUtc, CancellationToken cancellationToken = default);
}
