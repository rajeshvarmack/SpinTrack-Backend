using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.ProductVersion;

namespace SpinTrack.Application.Features.ProductVersions.Interfaces
{
    public interface IProductVersionRepository
    {
        Task<ProductVersion?> GetByIdAsync(Guid productVersionId, CancellationToken cancellationToken = default);
        Task<ProductVersion?> GetByProductAndVersionAsync(Guid productId, string versionNumber, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid productId, string versionNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<ProductVersion, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<ProductVersion, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(ProductVersion entity, CancellationToken cancellationToken = default);
        void Update(ProductVersion entity);
        void Delete(ProductVersion entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}