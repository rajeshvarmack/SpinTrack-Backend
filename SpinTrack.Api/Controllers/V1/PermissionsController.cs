using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.Permissions.DTOs;
using SpinTrack.Application.Features.Permissions.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PermissionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPermissionById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching permission with ID: {PermissionId}", id);
            var result = await _permissionService.GetPermissionByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission not found: {PermissionId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PermissionDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating permission: {PermissionKey}", request.PermissionKey);
            var result = await _permissionService.CreatePermissionAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create permission: {PermissionKey}", request.PermissionKey);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetPermissionById), new { id = result.Value.PermissionId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(PermissionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] UpdatePermissionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating permission: {PermissionId}", id);
            var result = await _permissionService.UpdatePermissionAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
            {
                if (result.Error?.Code == "ERROR.NOT_FOUND")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePermission(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting permission: {PermissionId}", id);
            var result = await _permissionService.DeletePermissionAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }

        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePermissionStatus(Guid id, [FromBody] ChangePermissionStatusRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Changing permission status: {PermissionId} to {Status}", id, request.Status);
            var result = await _permissionService.ChangePermissionStatusAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
            {
                if (result.Error?.Code == "ERROR.NOT_FOUND")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return Ok(new { message = "Permission status changed successfully" });
        }
    }
}