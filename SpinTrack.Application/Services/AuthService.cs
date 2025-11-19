using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Common.Settings;
using SpinTrack.Application.Features.Auth.DTOs;
using SpinTrack.Application.Features.Auth.Interfaces;
using SpinTrack.Application.Features.Auth.Mappers;
using SpinTrack.Application.Features.Users.Interfaces;
using SpinTrack.Core.Entities.Auth;
using BCrypt.Net;

namespace SpinTrack.Application.Services
{
    /// <summary>
    /// Authentication service implementation with account lockout
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly JwtSettings _jwtSettings;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenService jwtTokenService,
            ICurrentUserService currentUserService,
            IOptions<JwtSettings> jwtSettings,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator,
            IValidator<ChangePasswordRequest> changePasswordValidator,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenService = jwtTokenService;
            _currentUserService = currentUserService;
            _jwtSettings = jwtSettings.Value;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _changePasswordValidator = changePasswordValidator;
            _logger = logger;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request
            var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<AuthResponse>(Error.Validation("Validation failed", errors));
            }

            // Check if username exists
            if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken: cancellationToken))
            {
                return Result.Failure<AuthResponse>(Error.Conflict("Username already exists"));
            }

            // Check if email exists
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken: cancellationToken))
            {
                return Result.Failure<AuthResponse>(Error.Conflict("Email already exists"));
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user entity
            var user = UserMapper.ToEntity(request, passwordHash);

            // Add user to database
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User registered successfully: {UserId}, Username: {Username}", user.UserId, user.Username);

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            // Create response
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
                User = UserMapper.ToUserDto(user)
            };

            return Result.Success(response);
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request
            var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<AuthResponse>(Error.Validation("Validation failed", errors));
            }

            // Find user
            var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid username or password"));
            }

            // Check if account is locked
            if (user.IsLockedOut())
            {
                var remainingTime = user.LockoutEnd!.Value - DateTimeOffset.UtcNow;
                _logger.LogWarning("Login attempt for locked account: {Username}. Lockout ends in {Minutes} minutes", 
                    user.Username, remainingTime.Minutes);
                
                return Result.Failure<AuthResponse>(
                    Error.Forbidden($"Account is locked due to multiple failed login attempts. Please try again in {Math.Ceiling(remainingTime.TotalMinutes)} minutes."));
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                // Increment failed attempts
                user.IncrementFailedLoginAttempts(maxAttempts: 5, lockoutMinutes: 30);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync(cancellationToken);

                if (user.IsLockedOut())
                {
                    _logger.LogWarning("Account locked for user: {Username} after {Attempts} failed attempts", 
                        user.Username, user.FailedLoginAttempts);
                    
                    return Result.Failure<AuthResponse>(
                        Error.Forbidden("Account has been locked due to multiple failed login attempts. Please try again in 30 minutes."));
                }

                _logger.LogWarning("Failed login attempt {Attempt} for user: {Username}", 
                    user.FailedLoginAttempts, user.Username);
                
                return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid username or password"));
            }

            // Check if user is active
            if (user.Status != Core.Enums.UserStatus.Active)
            {
                return Result.Failure<AuthResponse>(Error.Forbidden($"User account is {user.Status}"));
            }

            // Reset failed attempts on successful login
            user.ResetFailedLoginAttempts();
            _userRepository.Update(user);

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successful login for user: {Username}", user.Username);

            // Create response
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
                User = UserMapper.ToUserDto(user)
            };

            return Result.Success(response);
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            // Find refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken == null)
            {
                return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid refresh token"));
            }

            // Check if token is active
            if (!refreshToken.IsActive)
            {
                return Result.Failure<AuthResponse>(Error.Unauthorized("Refresh token is expired or revoked"));
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure<AuthResponse>(Error.NotFound("User", refreshToken.UserId.ToString()));
            }

            // Check if user is active
            if (user.Status != Core.Enums.UserStatus.Active)
            {
                return Result.Failure<AuthResponse>(Error.Forbidden($"User account is {user.Status}"));
            }

            // Generate new tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            // Revoke old refresh token
            refreshToken.RevokedAt = DateTimeOffset.UtcNow;
            _refreshTokenRepository.Update(refreshToken);

            // Save new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = newRefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refresh token renewed for user: {UserId}", user.UserId);

            // Create response
            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
                User = UserMapper.ToUserDto(user)
            };

            return Result.Success(response);
        }

        public async Task<Result> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            // Find refresh token
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

            if (token == null)
            {
                return Result.Failure(Error.NotFound("RefreshToken", refreshToken));
            }

            // Revoke token
            token.RevokedAt = DateTimeOffset.UtcNow;
            _refreshTokenRepository.Update(token);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refresh token revoked: {TokenId}", token.RefreshTokenId);

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request
            var validationResult = await _changePasswordValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure(Error.Validation("Validation failed", errors));
            }

            // Get current user
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure(Error.Unauthorized("User not authenticated"));
            }

            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure(Error.NotFound("User", userId.Value.ToString()));
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return Result.Failure(Error.Unauthorized("Current password is incorrect"));
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Password changed successfully for user: {UserId}", userId.Value);

            return Result.Success();
        }
    }
}
