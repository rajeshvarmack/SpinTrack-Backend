using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Common.Services;
using SpinTrack.Application.Features.Auth.Mappers;
using SpinTrack.Application.Features.Users.DTOs;
using SpinTrack.Application.Features.Users.Interfaces;

namespace SpinTrack.Application.Services
{
    /// <summary>
    /// User profile service implementation
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IValidator<UpdateUserRequest> _updateUserValidator;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IFileStorageService fileStorageService,
            IValidator<UpdateUserRequest> updateUserValidator,
            ILogger<UserProfileService> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _fileStorageService = fileStorageService;
            _updateUserValidator = updateUserValidator;
            _logger = logger;
        }

        public async Task<Result<UserDetailDto>> GetCurrentUserProfileAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            
            if (!userId.HasValue)
            {
                return Result.Failure<UserDetailDto>(Error.Unauthorized("User not authenticated"));
            }

            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure<UserDetailDto>(Error.NotFound("User", userId.Value.ToString()));
            }

            var userDto = UserMapper.ToUserDetailDto(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDetailDto>> UpdateCurrentUserProfileAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
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

            var userId = _currentUserService.UserId;
            
            if (!userId.HasValue)
            {
                return Result.Failure<UserDetailDto>(Error.Unauthorized("User not authenticated"));
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure<UserDetailDto>(Error.NotFound("User", userId.Value.ToString()));
            }

            // Check if email exists (excluding current user)
            if (await _userRepository.EmailExistsAsync(request.Email, userId.Value, cancellationToken))
            {
                return Result.Failure<UserDetailDto>(Error.Conflict("Email already exists"));
            }

            // Update user
            UserMapper.UpdateEntity(user, request);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User profile updated: {UserId}", userId.Value);

            var userDto = UserMapper.ToUserDetailDto(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDetailDto>> UploadProfilePictureAsync(FileUpload file, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            
            if (!userId.HasValue)
            {
                return Result.Failure<UserDetailDto>(Error.Unauthorized("User not authenticated"));
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure<UserDetailDto>(Error.NotFound("User", userId.Value.ToString()));
            }

            // Delete old profile picture if exists
            if (!string.IsNullOrWhiteSpace(user.ProfilePicturePath))
            {
                await _fileStorageService.DeleteFileAsync(user.ProfilePicturePath, cancellationToken);
            }

            // Upload new profile picture
            var filePath = await _fileStorageService.UploadFileAsync(
                file.Content,
                file.FileName,
                file.ContentType,
                cancellationToken);

            user.ProfilePicturePath = filePath;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Profile picture uploaded for user: {UserId}", userId.Value);

            var userDto = UserMapper.ToUserDetailDto(user);
            return Result.Success(userDto);
        }

        public async Task<Result> DeleteProfilePictureAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            
            if (!userId.HasValue)
            {
                return Result.Failure(Error.Unauthorized("User not authenticated"));
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure(Error.NotFound("User", userId.Value.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(user.ProfilePicturePath))
            {
                await _fileStorageService.DeleteFileAsync(user.ProfilePicturePath, cancellationToken);
                user.ProfilePicturePath = null;
                
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Profile picture deleted for user: {UserId}", userId.Value);
            }

            return Result.Success();
        }
    }
}
