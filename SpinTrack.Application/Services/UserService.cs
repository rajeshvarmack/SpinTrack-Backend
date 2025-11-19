using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Auth.DTOs;
using SpinTrack.Application.Features.Auth.Mappers;
using SpinTrack.Application.Features.Users.DTOs;
using SpinTrack.Application.Features.Users.Interfaces;

namespace SpinTrack.Application.Services
{
    /// <summary>
    /// User service implementation with soft delete
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<UpdateUserRequest> _updateUserValidator;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<UpdateUserRequest> updateUserValidator,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _registerValidator = registerValidator;
            _updateUserValidator = updateUserValidator;
            _logger = logger;
        }

        public async Task<Result<UserDetailDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                return Result.Failure<UserDetailDto>(Error.NotFound("User", userId.ToString()));
            }

            var userDto = UserMapper.ToUserDetailDto(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDetailDto>> CreateUserAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request
            var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<UserDetailDto>(Error.Validation("Validation failed", errors));
            }

            // Check if username exists
            if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken: cancellationToken))
            {
                return Result.Failure<UserDetailDto>(Error.Conflict("Username already exists"));
            }

            // Check if email exists
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken: cancellationToken))
            {
                return Result.Failure<UserDetailDto>(Error.Conflict("Email already exists"));
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user entity
            var user = UserMapper.ToEntity(request, passwordHash);

            // Add user to database
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created successfully: {UserId}, Username: {Username}", user.UserId, user.Username);

            var userDto = UserMapper.ToUserDetailDto(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDetailDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request
            var validationResult = await _updateUserValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<UserDetailDto>(Error.Validation("Validation failed", errors));
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure<UserDetailDto>(Error.NotFound("User", userId.ToString()));
            }

            // Check if email exists (excluding current user)
            if (await _userRepository.EmailExistsAsync(request.Email, userId, cancellationToken))
            {
                return Result.Failure<UserDetailDto>(Error.Conflict("Email already exists"));
            }

            // Update user
            UserMapper.UpdateEntity(user, request);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User updated successfully: {UserId}", userId);

            var userDto = UserMapper.ToUserDetailDto(user);
            return Result.Success(userDto);
        }

        public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                return Result.Failure(Error.NotFound("User", userId.ToString()));
            }

            // Soft delete: Mark as deleted instead of removing from database
            user.IsDeleted = true;
            user.DeletedAt = DateTimeOffset.UtcNow;
            user.DeletedBy = _currentUserService.UserId;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User soft deleted successfully: {UserId} by {DeletedBy}", userId, _currentUserService.UserId);

            return Result.Success();
        }

        public async Task<Result> ChangeUserStatusAsync(Guid userId, ChangeUserStatusRequest request, CancellationToken cancellationToken = default)
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                return Result.Failure(Error.NotFound("User", userId.ToString()));
            }

            user.Status = request.Status;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User status changed successfully: {UserId}, New Status: {Status}", userId, request.Status);

            return Result.Success();
        }
    }
}
