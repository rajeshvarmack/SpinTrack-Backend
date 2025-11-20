using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.BusinessHolidays.DTOs;
using SpinTrack.Application.Features.BusinessHolidays.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class BusinessHolidaysController : ControllerBase
    {
        private readonly IBusinessHolidayService _businessHolidayService;
        private readonly ILogger<BusinessHolidaysController> _logger;

        public BusinessHolidaysController(IBusinessHolidayService businessHolidayService, ILogger<BusinessHolidaysController> logger)
        {
            _businessHolidayService = businessHolidayService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BusinessHolidayDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBusinessHolidayById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching business holiday with ID: {BusinessHolidayId}", id);
            var result = await _businessHolidayService.GetBusinessHolidayByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("BusinessHoliday not found: {BusinessHolidayId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BusinessHolidayDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBusinessHoliday([FromBody] CreateBusinessHolidayRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating business holiday for company: {CompanyId} date: {Date}", request.CompanyId, request.HolidayDate);
            var result = await _businessHolidayService.CreateBusinessHolidayAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create business holiday for company: {CompanyId} date: {Date}", request.CompanyId, request.HolidayDate);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetBusinessHolidayById), new { id = result.Value.BusinessHolidayId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BusinessHolidayDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateBusinessHoliday(Guid id, [FromBody] UpdateBusinessHolidayRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating business holiday: {BusinessHolidayId}", id);
            var result = await _businessHolidayService.UpdateBusinessHolidayAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteBusinessHoliday(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting business holiday: {BusinessHolidayId}", id);
            var result = await _businessHolidayService.DeleteBusinessHolidayAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}