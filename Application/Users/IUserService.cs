using Application.Users.Models;

namespace Application.Users;

public interface IUserService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<RevokeTokenResponse> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default);
    Task<LogoutResponse> LogoutAsync(long userId, CancellationToken cancellationToken = default);
    Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<UpdateUserResponse> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<DeleteUserResponse> DeleteAsync(long userId, CancellationToken cancellationToken = default);
    Task<ActivateUserResponse> ActivateAsync(long userId, CancellationToken cancellationToken = default);
}
