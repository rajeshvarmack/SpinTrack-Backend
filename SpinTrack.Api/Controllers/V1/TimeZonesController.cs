using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.TimeZones.DTOs;
using SpinTrack.Application.Features.TimeZones.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TimeZonesController : ControllerBase
    {
        private readonly ITimeZoneService _timeZoneService;
        private readonly ILogger<TimeZonesController> _logger;

        public TimeZonesController(ITimeZoneService timeZoneService, ILogger<TimeZonesController> logger)
        {
            _timeZoneService = timeZoneService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TimeZoneDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTimeZoneById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching timezone with ID: {TimeZoneId}", id);
            var result = await _timeZoneService.GetTimeZoneByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("TimeZone not found: {TimeZoneId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TimeZoneDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTimeZone([FromBody] CreateTimeZoneRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating timezone: {TimeZoneName}", request.TimeZoneName);
            var result = await _timeZoneService.CreateTimeZoneAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create timezone: {TimeZoneName}", request.TimeZoneName);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetTimeZoneById), new { id = result.Value.TimeZoneId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TimeZoneDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateTimeZone(Guid id, [FromBody] UpdateTimeZoneRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating timezone: {TimeZoneId}", id);
            var result = await _timeZoneService.UpdateTimeZoneAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteTimeZone(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting timezone: {TimeZoneId}", id);
            var result = await _timeZoneService.DeleteTimeZoneAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}