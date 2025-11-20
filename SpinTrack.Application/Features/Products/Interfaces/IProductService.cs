using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Products.DTOs;

namespace SpinTrack.Application.Features.Products.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDetailDto>> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<Result<ProductDetailDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
        Task<Result<ProductDetailDto>> UpdateProductAsync(Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteProductAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}