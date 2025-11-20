using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.SubModules.DTOs;
using SpinTrack.Application.Features.SubModules.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class SubModulesController : ControllerBase
    {
        private readonly ISubModuleService _subModuleService;
        private readonly ILogger<SubModulesController> _logger;

        public SubModulesController(ISubModuleService subModuleService, ILogger<SubModulesController> logger)
        {
            _subModuleService = subModuleService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SubModuleDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSubModuleById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching submodule with ID: {SubModuleId}", id);
            var result = await _subModuleService.GetSubModuleByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("SubModule not found: {SubModuleId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SubModuleDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateSubModule([FromBody] CreateSubModuleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating submodule: {SubModuleKey}", request.SubModuleKey);
            var result = await _subModuleService.CreateSubModuleAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create submodule: {SubModuleKey}", request.SubModuleKey);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetSubModuleById), new { id = result.Value.SubModuleId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(SubModuleDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateSubModule(Guid id, [FromBody] UpdateSubModuleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating submodule: {SubModuleId}", id);
            var result = await _subModuleService.UpdateSubModuleAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteSubModule(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting submodule: {SubModuleId}", id);
            var result = await _subModuleService.DeleteSubModuleAsync(id, cancellationToken);
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
        public async Task<IActionResult> ChangeSubModuleStatus(Guid id, [FromBody] ChangeSubModuleStatusRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Changing submodule status: {SubModuleId} to {Status}", id, request.Status);
            var result = await _subModuleService.ChangeSubModuleStatusAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
            {
                if (result.Error?.Code == "ERROR.NOT_FOUND")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return Ok(new { message = "SubModule status changed successfully" });
        }
    }
}