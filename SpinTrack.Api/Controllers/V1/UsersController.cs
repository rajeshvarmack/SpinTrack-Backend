using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Auth.DTOs;
using SpinTrack.Application.Features.Users.DTOs;
using SpinTrack.Application.Features.Users.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    /// <summary>
    /// User management controller for admin operations
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserQueryService _userQueryService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            IUserQueryService userQueryService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _userQueryService = userQueryService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of users with filtering, searching, and sorting
        /// </summary>
        /// <param name="request">Query parameters including pagination, filters, search, and sorting</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of users</returns>
        /// <response code="200">Returns paginated user list</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("query")]
        [ProducesResponseType(typeof(PagedResult<UserQueryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> QueryUsers([FromBody] QueryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching users with query parameters");

            var result = await _userQueryService.QueryUsersAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch users. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully fetched {Count} users", result.Value.Items.Count);
            return Ok(result.Value);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User details</returns>
        /// <response code="200">Returns user details</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            var result = await _userService.GetUserByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                return NotFound(result.Error);
            }

            _logger.LogInformation("Successfully fetched user: {UserId}", id);
            return Ok(result.Value);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="request">User creation details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created user details</returns>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="409">User already exists</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new user: {Username}", request.Username);

            var result = await _userService.CreateUserAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create user: {Username}. Error: {Error}",
                    request.Username, result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("User created successfully: {UserId}", result.Value.UserId);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Value.UserId }, result.Value);
        }

        /// <summary>
        /// Update user details
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="request">Updated user details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated user details</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating user: {UserId}", id);

            var result = await _userService.UpdateUserAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update user: {UserId}. Error: {Error}",
                    id, result.Error?.Message);

                if (result.Error?.Code == "ERROR.NOT_FOUND")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            _logger.LogInformation("User updated successfully: {UserId}", id);
            return Ok(result.Value);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        /// <response code="204">User deleted successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting user: {UserId}", id);

            var result = await _userService.DeleteUserAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete user: {UserId}. Error: {Error}",
                    id, result.Error?.Message);
                return NotFound(result.Error);
            }

            _logger.LogInformation("User deleted successfully: {UserId}", id);
            return NoContent();
        }

        /// <summary>
        /// Change user status (Active/Inactive/Suspended)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="request">New status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        /// <response code="200">Status changed successfully</response>
        /// <response code="400">Invalid status</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeUserStatus(Guid id, [FromBody] ChangeUserStatusRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Changing status for user: {UserId} to {Status}", id, request.Status);

            var result = await _userService.ChangeUserStatusAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to change user status: {UserId}. Error: {Error}",
                    id, result.Error?.Message);

                if (result.Error?.Code == "ERROR.NOT_FOUND")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            _logger.LogInformation("User status changed successfully: {UserId}", id);
            return Ok(new { message = "User status changed successfully" });
        }

        /// <summary>
        /// Export users to CSV based on current query
        /// </summary>
        /// <param name="request">Query parameters for export</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File download</returns>
        /// <response code="200">Returns file for download</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportUsers([FromBody] QueryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exporting users with format: {Format}", request.ExportFormat);

            var result = await _userQueryService.ExportUsersAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to export users. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully exported users");

            var exportResult = result.Value;
            return File(exportResult.FileContent, exportResult.ContentType, exportResult.FileName);
        }
    }
}
