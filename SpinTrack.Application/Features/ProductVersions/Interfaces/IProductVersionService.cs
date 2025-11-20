using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.ProductVersions.DTOs;

namespace SpinTrack.Application.Features.ProductVersions.Interfaces
{
    public interface IProductVersionService
    {
        Task<Result<ProductVersionDetailDto>> GetProductVersionByIdAsync(Guid productVersionId, CancellationToken cancellationToken = default);
        Task<Result<ProductVersionDetailDto>> CreateProductVersionAsync(CreateProductVersionRequest request, CancellationToken cancellationToken = default);
        Task<Result<ProductVersionDetailDto>> UpdateProductVersionAsync(Guid productVersionId, UpdateProductVersionRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteProductVersionAsync(Guid productVersionId, CancellationToken cancellationToken = default);
    }
}