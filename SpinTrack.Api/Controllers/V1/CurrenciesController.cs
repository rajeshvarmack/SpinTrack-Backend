using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.Currencies.DTOs;
using SpinTrack.Application.Features.Currencies.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<CurrenciesController> _logger;

        public CurrenciesController(ICurrencyService currencyService, ILogger<CurrenciesController> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CurrencyDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrencyById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching currency with ID: {CurrencyId}", id);
            var result = await _currencyService.GetCurrencyByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Currency not found: {CurrencyId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CurrencyDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCurrency([FromBody] CreateCurrencyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating currency: {CurrencyCode}", request.CurrencyCode);
            var result = await _currencyService.CreateCurrencyAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create currency: {CurrencyCode}", request.CurrencyCode);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetCurrencyById), new { id = result.Value.CurrencyId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CurrencyDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCurrency(Guid id, [FromBody] UpdateCurrencyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating currency: {CurrencyId}", id);
            var result = await _currencyService.UpdateCurrencyAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteCurrency(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting currency: {CurrencyId}", id);
            var result = await _currencyService.DeleteCurrencyAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}