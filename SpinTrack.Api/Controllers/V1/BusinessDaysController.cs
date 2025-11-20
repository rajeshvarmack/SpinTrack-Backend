using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.BusinessDays.DTOs;
using SpinTrack.Application.Features.BusinessDays.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class BusinessDaysController : ControllerBase
    {
        private readonly IBusinessDayService _businessDayService;
        private readonly ILogger<BusinessDaysController> _logger;

        public BusinessDaysController(IBusinessDayService businessDayService, ILogger<BusinessDaysController> logger)
        {
            _businessDayService = businessDayService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BusinessDayDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBusinessDayById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching business day with ID: {BusinessDayId}", id);
            var result = await _businessDayService.GetBusinessDayByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("BusinessDay not found: {BusinessDayId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BusinessDayDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBusinessDay([FromBody] CreateBusinessDayRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating business day for company: {CompanyId} day: {Day}", request.CompanyId, request.DayOfWeek);
            var result = await _businessDayService.CreateBusinessDayAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create business day for company: {CompanyId} day: {Day}", request.CompanyId, request.DayOfWeek);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetBusinessDayById), new { id = result.Value.BusinessDayId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BusinessDayDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateBusinessDay(Guid id, [FromBody] UpdateBusinessDayRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating business day: {BusinessDayId}", id);
            var result = await _businessDayService.UpdateBusinessDayAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteBusinessDay(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting business day: {BusinessDayId}", id);
            var result = await _businessDayService.DeleteBusinessDayAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}