using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.ProductVersions.DTOs;
using SpinTrack.Application.Features.ProductVersions.Interfaces;
using SpinTrack.Application.Features.ProductVersions.Mappers;
using SpinTrack.Core.Entities.ProductVersion;

namespace SpinTrack.Application.Services
{
    public class ProductVersionService : IProductVersionService
    {
        private readonly IProductVersionRepository _productVersionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateProductVersionRequest> _createValidator;
        private readonly IValidator<UpdateProductVersionRequest> _updateValidator;
        private readonly ILogger<ProductVersionService> _logger;

        public ProductVersionService(
            IProductVersionRepository productVersionRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateProductVersionRequest> createValidator,
            IValidator<UpdateProductVersionRequest> updateValidator,
            ILogger<ProductVersionService> logger)
        {
            _productVersionRepository = productVersionRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<ProductVersionDetailDto>> GetProductVersionByIdAsync(Guid productVersionId, CancellationToken cancellationToken = default)
        {
            var pv = await _productVersionRepository.GetByIdAsync(productVersionId, cancellationToken);
            if (pv == null)
                return Result.Failure<ProductVersionDetailDto>(Error.NotFound("ProductVersion", productVersionId.ToString()));

            return Result.Success(ProductVersionMapper.ToProductVersionDetailDto(pv));
        }

        public async Task<Result<ProductVersionDetailDto>> CreateProductVersionAsync(CreateProductVersionRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<ProductVersionDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _productVersionRepository.ExistsAsync(request.ProductId, request.VersionNumber, cancellationToken: cancellationToken))
            {
                return Result.Failure<ProductVersionDetailDto>(Error.Conflict("Product version already exists for this product"));
            }

            var pv = ProductVersionMapper.ToEntity(request);
            await _productVersionRepository.AddAsync(pv, cancellationToken);
            await _productVersionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ProductVersion created: {ProductVersionId} Product: {ProductId} Version: {Version}", pv.ProductVersionId, pv.ProductId, pv.VersionNumber);
            return Result.Success(ProductVersionMapper.ToProductVersionDetailDto(pv));
        }

        public async Task<Result<ProductVersionDetailDto>> UpdateProductVersionAsync(Guid productVersionId, UpdateProductVersionRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<ProductVersionDetailDto>(Error.Validation("Validation failed", errors));
            }

            var pv = await _productVersionRepository.GetByIdAsync(productVersionId, cancellationToken);
            if (pv == null)
                return Result.Failure<ProductVersionDetailDto>(Error.NotFound("ProductVersion", productVersionId.ToString()));

            ProductVersionMapper.UpdateEntity(pv, request);
            _productVersionRepository.Update(pv);
            await _productVersionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ProductVersion updated: {ProductVersionId}", productVersionId);
            return Result.Success(ProductVersionMapper.ToProductVersionDetailDto(pv));
        }

        public async Task<Result> DeleteProductVersionAsync(Guid productVersionId, CancellationToken cancellationToken = default)
        {
            var pv = await _productVersionRepository.GetByIdAsync(productVersionId, cancellationToken);
            if (pv == null)
                return Result.Failure(Error.NotFound("ProductVersion", productVersionId.ToString()));

            pv.IsDeleted = true;
            _productVersionRepository.Update(pv);
            await _productVersionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ProductVersion soft deleted: {ProductVersionId}", productVersionId);
            return Result.Success();
        }
    }
}
