using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Product;

namespace SpinTrack.Application.Features.Products.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<Product?> GetByCodeAsync(string productCode, CancellationToken cancellationToken = default);
        Task<bool> ProductCodeExistsAsync(string productCode, Guid? excludeProductId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Product, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Product, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        void Update(Product product);
        void Delete(Product product);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}