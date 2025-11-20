using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpinTrack.Application.Features.ProductVersions.DTOs;
using SpinTrack.Application.Features.ProductVersions.Interfaces;

namespace SpinTrack.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ProductVersionsController : ControllerBase
    {
        private readonly IProductVersionService _productVersionService;
        private readonly ILogger<ProductVersionsController> _logger;

        public ProductVersionsController(IProductVersionService productVersionService, ILogger<ProductVersionsController> logger)
        {
            _productVersionService = productVersionService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductVersionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProductVersionById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching product version with ID: {ProductVersionId}", id);
            var result = await _productVersionService.GetProductVersionByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("ProductVersion not found: {ProductVersionId}", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductVersionDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProductVersion([FromBody] CreateProductVersionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating product version for product: {ProductId} version: {Version}", request.ProductId, request.VersionNumber);
            var result = await _productVersionService.CreateProductVersionAsync(request, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create product version for product: {ProductId} version: {Version}", request.ProductId, request.VersionNumber);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetProductVersionById), new { id = result.Value.ProductVersionId }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductVersionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProductVersion(Guid id, [FromBody] UpdateProductVersionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating product version: {ProductVersionId}", id);
            var result = await _productVersionService.UpdateProductVersionAsync(id, request, cancellationToken);
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
        public async Task<IActionResult> DeleteProductVersion(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting product version: {ProductVersionId}", id);
            var result = await _productVersionService.DeleteProductVersionAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
    }
}