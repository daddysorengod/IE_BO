using Application.Security;
using Application.Users.Models;
using Application.Users.Repositories;
using Domain.Users;
using Microsoft.Extensions.Logging;

namespace Application.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly ISuperUserSessionStore _superUserSessionStore;
    private readonly JwtOptions _jwtOptions;
    private readonly SuperUserOptions _superUserOptions;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        ISuperUserSessionStore superUserSessionStore,
        JwtOptions jwtOptions,
        SuperUserOptions superUserOptions,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _superUserSessionStore = superUserSessionStore;
        _jwtOptions = jwtOptions;
        _superUserOptions = superUserOptions;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateLoginRequest(request);

            var normalizedUsername = request.Username.Trim();
            if (IsConfiguredSuperUserLogin(normalizedUsername, request.Password))
            {
                return await IssueTokensAsync(CreateConfiguredSuperUser(), cancellationToken);
            }

            var user = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);
            if (user is null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("User is not active.");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return null;
            }

            return await IssueTokensAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoginAsync failed for username: {Username}", request.Username);
            throw;
        }
    }

    public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRefreshTokenRequest(request);

            var refreshToken = request.RefreshToken.Trim();
            if (_superUserSessionStore.IsValidRefreshToken(refreshToken))
            {
                return await IssueTokensAsync(CreateConfiguredSuperUser(), cancellationToken);
            }

            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
            if (user is null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(user.RefreshToken)
                || !string.Equals(user.RefreshToken, refreshToken, StringComparison.Ordinal)
                || user.RefreshTokenExpiresAtUtc is null
                || user.RefreshTokenExpiresAtUtc <= DateTime.UtcNow)
            {
                return null;
            }

            return await IssueTokensAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RefreshTokenAsync failed.");
            throw;
        }
    }

    public async Task<RevokeTokenResponse> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRevokeTokenRequest(request);

            var refreshToken = request.RefreshToken.Trim();
            if (_superUserSessionStore.IsValidRefreshToken(refreshToken))
            {
                _superUserSessionStore.Clear();
                return new RevokeTokenResponse
                {
                    Id = _superUserOptions.UserId,
                    IsRevoked = true
                };
            }

            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
            if (user is null)
            {
                return new RevokeTokenResponse
                {
                    Id = 0,
                    IsRevoked = false
                };
            }

            var revoked = await _userRepository.UpdateRefreshTokenAsync(user.Id, null, null, cancellationToken);
            return new RevokeTokenResponse
            {
                Id = user.Id,
                IsRevoked = revoked
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RevokeTokenAsync failed.");
            throw;
        }
    }

    public async Task<LogoutResponse> LogoutAsync(long userId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateUserId(userId);

            if (IsConfiguredSuperUserId(userId))
            {
                var hadValidSession = _superUserSessionStore.HasValidSession();
                _superUserSessionStore.Clear();

                return new LogoutResponse
                {
                    Id = userId,
                    IsLoggedOut = hadValidSession
                };
            }

            var loggedOut = await _userRepository.UpdateRefreshTokenAsync(userId, null, null, cancellationToken);
            return new LogoutResponse
            {
                Id = userId,
                IsLoggedOut = loggedOut
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogoutAsync failed for user id: {Id}", userId);
            throw;
        }
    }

    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRegisterRequest(request);

            var normalizedUsername = request.Username.Trim();
            var existingUsernameUser = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);
            if (existingUsernameUser is not null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var normalizedEmail = request.Email.Trim();
            var existingEmailUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (existingEmailUser is not null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var user = new User
            {
                Username = normalizedUsername,
                Email = normalizedEmail,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FullName = request.FullName.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
                Role = request.Role.Trim(),
                IsActive = false
            };

            var userId = await _userRepository.InsertAsync(user, cancellationToken);

            return new RegisterUserResponse
            {
                Id = userId,
                IsActive = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RegisterAsync failed for username: {Username}", request.Username);
            throw;
        }
    }

    public async Task<UpdateUserResponse> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateUpdateRequest(request);

            var existingUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingUser is null)
            {
                return new UpdateUserResponse
                {
                    Id = request.Id,
                    IsUpdated = false
                };
            }

            var normalizedUsername = request.Username.Trim();
            var existingUsernameUser = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);
            if (existingUsernameUser is not null && existingUsernameUser.Id != request.Id)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var normalizedEmail = request.Email.Trim();
            var existingEmailUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (existingEmailUser is not null && existingEmailUser.Id != request.Id)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            existingUser.Username = normalizedUsername;
            existingUser.Email = normalizedEmail;
            existingUser.FullName = request.FullName.Trim();
            existingUser.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            existingUser.Role = request.Role.Trim();

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(request.Password);
            }

            var updated = await _userRepository.UpdateAsync(existingUser, cancellationToken);
            return new UpdateUserResponse
            {
                Id = request.Id,
                IsUpdated = updated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed for user id: {Id}", request.Id);
            throw;
        }
    }

    public async Task<DeleteUserResponse> DeleteAsync(long userId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateUserId(userId);

            var deleted = await _userRepository.DeleteAsync(userId, cancellationToken);
            return new DeleteUserResponse
            {
                Id = userId,
                IsDeleted = deleted
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAsync failed for user id: {Id}", userId);
            throw;
        }
    }

    public async Task<ActivateUserResponse> ActivateAsync(long userId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateUserId(userId);

            var activated = await _userRepository.ActivateAsync(userId, cancellationToken);
            return new ActivateUserResponse
            {
                Id = userId,
                IsActivated = activated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ActivateAsync failed for user id: {Id}", userId);
            throw;
        }
    }

    private static void ValidateLoginRequest(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }
    }

    private static void ValidateRegisterRequest(RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException("FullName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Role))
        {
            throw new ArgumentException("Role is required.");
        }
    }

    private static void ValidateUpdateRequest(UpdateUserRequest request)
    {
        ValidateUserId(request.Id);

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException("FullName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Role))
        {
            throw new ArgumentException("Role is required.");
        }
    }

    private static void ValidateUserId(long userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Id must be greater than 0.");
        }
    }

    private static void ValidateRefreshTokenRequest(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new ArgumentException("RefreshToken is required.");
        }
    }

    private static void ValidateRevokeTokenRequest(RevokeTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new ArgumentException("RefreshToken is required.");
        }
    }

    private async Task<LoginResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _accessTokenGenerator.Generate(user);
        var refreshToken = _refreshTokenGenerator.Generate();
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        if (IsConfiguredSuperUser(user))
        {
            _superUserSessionStore.Save(refreshToken, refreshTokenExpiresAtUtc);
        }
        else
        {
            await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiresAtUtc, cancellationToken);
        }

        return new LoginResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            ImageUrl = user.ImageUrl,
            Role = user.Role,
            TokenType = "Bearer",
            AccessToken = accessToken.Token,
            AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            IsActive = user.IsActive
        };
    }

    private bool IsConfiguredSuperUserLogin(string username, string password)
    {
        return _superUserOptions.IsEnabled()
            && string.Equals(username, _superUserOptions.SupperUser, StringComparison.Ordinal)
            && string.Equals(password, _superUserOptions.SupperPassword, StringComparison.Ordinal);
    }

    private bool IsConfiguredSuperUser(User user)
    {
        return _superUserOptions.IsEnabled()
            && user.Id == _superUserOptions.UserId
            && string.Equals(user.Username, _superUserOptions.SupperUser, StringComparison.Ordinal);
    }

    private bool IsConfiguredSuperUserId(long userId)
    {
        return _superUserOptions.IsEnabled() && userId == _superUserOptions.UserId;
    }

    private User CreateConfiguredSuperUser()
    {
        return new User
        {
            Id = _superUserOptions.UserId,
            Username = _superUserOptions.SupperUser,
            Email = _superUserOptions.SupperUser,
            PasswordHash = string.Empty,
            FullName = _superUserOptions.FullName,
            ImageUrl = _superUserOptions.ImageUrl,
            Role = _superUserOptions.Role,
            IsActive = true
        };
    }
}
