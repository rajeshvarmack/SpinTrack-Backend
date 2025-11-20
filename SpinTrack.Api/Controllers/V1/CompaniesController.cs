using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.Companies.DTOs;
using SpinTrack.Application.Features.Companies.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CompanyDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCompanyById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching company with ID: {CompanyId}", id);
            var result = await _companyService.GetCompanyByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Company not found: {CompanyId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CompanyDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating company: {CompanyName}", request.CompanyName);
            var result = await _companyService.CreateCompanyAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create company: {CompanyName}", request.CompanyName);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetCompanyById), new { id = result.Value.CompanyId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CompanyDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating company: {CompanyId}", id);
            var result = await _companyService.UpdateCompanyAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteCompany(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting company: {CompanyId}", id);
            var result = await _companyService.DeleteCompanyAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}