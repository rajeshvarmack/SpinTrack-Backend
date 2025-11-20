using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.Modules.DTOs;
using SpinTrack.Application.Features.Modules.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly ILogger<ModulesController> _logger;

        public ModulesController(IModuleService moduleService, ILogger<ModulesController> logger)
        {
            _moduleService = moduleService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ModuleDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetModuleById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching module with ID: {ModuleId}", id);
            var result = await _moduleService.GetModuleByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Module not found: {ModuleId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ModuleDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating module: {ModuleKey}", request.ModuleKey);
            var result = await _moduleService.CreateModuleAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create module: {ModuleKey}", request.ModuleKey);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetModuleById), new { id = result.Value.ModuleId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ModuleDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateModule(Guid id, [FromBody] UpdateModuleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating module: {ModuleId}", id);
            var result = await _moduleService.UpdateModuleAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteModule(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting module: {ModuleId}", id);
            var result = await _moduleService.DeleteModuleAsync(id, cancellationToken);
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
        public async Task<IActionResult> ChangeModuleStatus(Guid id, [FromBody] ChangeModuleStatusRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Changing module status: {ModuleId} to {Status}", id, request.Status);
            var result = await _moduleService.ChangeModuleStatusAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
            {
                if (result.Error?.Code == "ERROR.NOT_FOUND")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return Ok(new { message = "Module status changed successfully" });
        }
    }
}