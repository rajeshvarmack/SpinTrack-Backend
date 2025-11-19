using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.Auth.DTOs;
using SpinTrack.Application.Features.Auth.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    /// <summary>
    /// Authentication controller for user registration, login, and token management
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with tokens</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="409">User already exists</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User registration attempt for username: {Username}", request.Username);

            var result = await _authService.RegisterAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User registration failed for username: {Username}. Error: {Error}",
                    request.Username, result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("User registered successfully: {Username}", request.Username);
            return Ok(result.Value);
        }

        /// <summary>
        /// Login with username and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with tokens</returns>
        /// <response code="200">Login successful</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for username: {Username}", request.Username);

            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Login failed for username: {Username}. Error: {Error}",
                    request.Username, result.Error?.Message);
                return Unauthorized(result.Error);
            }

            _logger.LogInformation("User logged in successfully: {Username}", request.Username);
            return Ok(result.Value);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>New authentication tokens</returns>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="400">Invalid or expired refresh token</response>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refresh token attempt");

            var result = await _authService.RefreshTokenAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Token refresh failed. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Token refreshed successfully");
            return Ok(result.Value);
        }

        /// <summary>
        /// Revoke refresh token (logout)
        /// </summary>
        /// <param name="request">Refresh token to revoke</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success status</returns>
        /// <response code="200">Token revoked successfully</response>
        /// <response code="400">Invalid token</response>
        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Revoke token attempt");

            var result = await _authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Token revocation failed. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Token revoked successfully");
            return Ok(new { message = "Token revoked successfully" });
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        /// <param name="request">Current and new password</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status message</returns>
        /// <response code="200">Password changed successfully</response>
        /// <response code="400">Invalid current password</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Password change attempt for user");

            var result = await _authService.ChangePasswordAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password change failed. Error: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Password changed successfully");
            return Ok(new { message = "Password changed successfully" });
        }
    }
}
