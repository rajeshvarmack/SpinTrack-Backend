using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Products.DTOs;
using SpinTrack.Application.Features.Products.Interfaces;
using SpinTrack.Application.Features.Products.Mappers;
using SpinTrack.Core.Entities.Product;

namespace SpinTrack.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateProductRequest> _createValidator;
        private readonly IValidator<UpdateProductRequest> _updateValidator;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateProductRequest> createValidator,
            IValidator<UpdateProductRequest> updateValidator,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<ProductDetailDto>> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var p = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (p == null)
                return Result.Failure<ProductDetailDto>(Error.NotFound("Product", productId.ToString()));

            return Result.Success(ProductMapper.ToProductDetailDto(p));
        }

        public async Task<Result<ProductDetailDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<ProductDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _productRepository.ProductCodeExistsAsync(request.ProductCode, cancellationToken: cancellationToken))
            {
                return Result.Failure<ProductDetailDto>(Error.Conflict("Product code already exists"));
            }

            var p = ProductMapper.ToEntity(request);
            await _productRepository.AddAsync(p, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product created: {ProductId} Code: {Code}", p.ProductId, p.ProductCode);
            return Result.Success(ProductMapper.ToProductDetailDto(p));
        }

        public async Task<Result<ProductDetailDto>> UpdateProductAsync(Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<ProductDetailDto>(Error.Validation("Validation failed", errors));
            }

            var p = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (p == null)
                return Result.Failure<ProductDetailDto>(Error.NotFound("Product", productId.ToString()));

            ProductMapper.UpdateEntity(p, request);
            _productRepository.Update(p);
            await _productRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product updated: {ProductId}", productId);
            return Result.Success(ProductMapper.ToProductDetailDto(p));
        }

        public async Task<Result> DeleteProductAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var p = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (p == null)
                return Result.Failure(Error.NotFound("Product", productId.ToString()));

            p.IsDeleted = true;
            _productRepository.Update(p);
            await _productRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product soft deleted: {ProductId}", productId);
            return Result.Success();
        }
    }
}
