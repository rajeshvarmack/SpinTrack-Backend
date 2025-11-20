using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.DateFormats.DTOs;
using SpinTrack.Application.Features.DateFormats.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class DateFormatsController : ControllerBase
    {
        private readonly IDateFormatService _dateFormatService;
        private readonly ILogger<DateFormatsController> _logger;

        public DateFormatsController(IDateFormatService dateFormatService, ILogger<DateFormatsController> logger)
        {
            _dateFormatService = dateFormatService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DateFormatDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDateFormatById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching date format with ID: {DateFormatId}", id);
            var result = await _dateFormatService.GetDateFormatByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("DateFormat not found: {DateFormatId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DateFormatDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateDateFormat([FromBody] CreateDateFormatRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating date format: {Format}", request.FormatString);
            var result = await _dateFormatService.CreateDateFormatAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create date format: {Format}", request.FormatString);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetDateFormatById), new { id = result.Value.DateFormatId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DateFormatDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateDateFormat(Guid id, [FromBody] UpdateDateFormatRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating date format: {DateFormatId}", id);
            var result = await _dateFormatService.UpdateDateFormatAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteDateFormat(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting date format: {DateFormatId}", id);
            var result = await _dateFormatService.DeleteDateFormatAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}