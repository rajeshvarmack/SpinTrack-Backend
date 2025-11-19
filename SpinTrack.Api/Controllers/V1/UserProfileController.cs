using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Users.DTOs;
using SpinTrack.Application.Features.Users.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    /// <summary>
    /// User profile controller for managing current user's profile
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IUserProfileService userProfileService,
            ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        /// <returns>Current user's profile details</returns>
        /// <response code="200">Returns user profile</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User not found</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching current user profile");

            var result = await _userProfileService.GetCurrentUserProfileAsync(cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch user profile. Error: {Error}", result.Error?.Message);
                return NotFound(result.Error);
            }

            _logger.LogInformation("Successfully fetched user profile");
            return Ok(result.Value);
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        /// <param name="request">Updated profile details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated profile</returns>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut("me")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating current user profile");

            var result = await _userProfileService.UpdateCurrentUserProfileAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update user profile. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully updated user profile");
            return Ok(result.Value);
        }

        /// <summary>
        /// Upload profile picture for current user (Local Storage)
        /// </summary>
        /// <param name="file">Profile picture file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated profile with picture path</returns>
        /// <response code="200">Profile picture uploaded successfully</response>
        /// <response code="400">Invalid file</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("me/profile-picture/local")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadProfilePictureLocal(IFormFile file, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading profile picture to local storage");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file provided for profile picture upload");
                return BadRequest(new { message = "No file provided" });
            }

            // Validate file type and size
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid file type for profile picture: {Extension}", extension);
                return BadRequest(new { message = "Only image files (jpg, jpeg, png, gif) are allowed" });
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                _logger.LogWarning("File size too large: {Size} bytes", file.Length);
                return BadRequest(new { message = "File size must not exceed 5MB" });
            }

            // Convert IFormFile to FileUpload
            var fileUpload = new FileUpload
            {
                Content = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length
            };

            var result = await _userProfileService.UploadProfilePictureAsync(fileUpload, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to upload profile picture. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully uploaded profile picture to local storage");
            return Ok(result.Value);
        }

        /// <summary>
        /// Upload profile picture for current user (Azure Blob Storage)
        /// </summary>
        /// <param name="file">Profile picture file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated profile with picture path</returns>
        /// <response code="200">Profile picture uploaded successfully</response>
        /// <response code="400">Invalid file</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("me/profile-picture/azure")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadProfilePictureAzure(IFormFile file, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading profile picture to Azure Blob Storage");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file provided for profile picture upload");
                return BadRequest(new { message = "No file provided" });
            }

            // Validate file type and size
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid file type for profile picture: {Extension}", extension);
                return BadRequest(new { message = "Only image files (jpg, jpeg, png, gif) are allowed" });
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                _logger.LogWarning("File size too large: {Size} bytes", file.Length);
                return BadRequest(new { message = "File size must not exceed 5MB" });
            }

            // Convert IFormFile to FileUpload
            var fileUpload = new FileUpload
            {
                Content = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length
            };

            var result = await _userProfileService.UploadProfilePictureAsync(fileUpload, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to upload profile picture. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully uploaded profile picture to Azure Blob Storage");
            return Ok(result.Value);
        }

        /// <summary>
        /// Delete current user's profile picture
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        /// <response code="200">Profile picture deleted successfully</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete("me/profile-picture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProfilePicture(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting profile picture");

            var result = await _userProfileService.DeleteProfilePictureAsync(cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete profile picture. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully deleted profile picture");
            return Ok(new { message = "Profile picture deleted successfully" });
        }
    }
}
