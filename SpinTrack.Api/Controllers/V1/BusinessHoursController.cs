using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.BusinessHours.DTOs;
using SpinTrack.Application.Features.BusinessHours.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class BusinessHoursController : ControllerBase
    {
        private readonly IBusinessHoursService _businessHoursService;
        private readonly ILogger<BusinessHoursController> _logger;

        public BusinessHoursController(IBusinessHoursService businessHoursService, ILogger<BusinessHoursController> logger)
        {
            _businessHoursService = businessHoursService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BusinessHoursDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBusinessHoursById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching business hours with ID: {BusinessHoursId}", id);
            var result = await _businessHoursService.GetBusinessHoursByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("BusinessHours not found: {BusinessHoursId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BusinessHoursDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBusinessHours([FromBody] CreateBusinessHoursRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating business hours for company: {CompanyId} day: {Day}", request.CompanyId, request.DayOfWeek);
            var result = await _businessHoursService.CreateBusinessHoursAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create business hours for company: {CompanyId} day: {Day}", request.CompanyId, request.DayOfWeek);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetBusinessHoursById), new { id = result.Value.BusinessHoursId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BusinessHoursDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateBusinessHours(Guid id, [FromBody] UpdateBusinessHoursRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating business hours: {BusinessHoursId}", id);
            var result = await _businessHoursService.UpdateBusinessHoursAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteBusinessHours(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting business hours: {BusinessHoursId}", id);
            var result = await _businessHoursService.DeleteBusinessHoursAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}