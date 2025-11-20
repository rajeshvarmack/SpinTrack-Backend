using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Products.Interfaces;
using SpinTrack.Core.Entities.Product;

namespace SpinTrack.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SpinTrackDbContext _context;

        public ProductRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Product>().AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
        }

        public async Task<Product?> GetByCodeAsync(string productCode, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Product>().AsNoTracking().FirstOrDefaultAsync(p => p.ProductCode == productCode, cancellationToken);
        }

        public async Task<bool> ProductCodeExistsAsync(string productCode, Guid? excludeProductId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Product>().AsNoTracking().Where(p => p.ProductCode == productCode);
            if (excludeProductId.HasValue)
                query = query.Where(p => p.ProductId != excludeProductId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Product, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Product>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(p => p.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Product, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Product>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(p => p.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Set<Product>().AddAsync(product, cancellationToken);
        }

        public void Update(Product product)
        {
            _context.Set<Product>().Update(product);
        }

        public void Delete(Product product)
        {
            _context.Set<Product>().Remove(product);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<Product> ApplySorting(IQueryable<Product> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<Product>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "productid" => desc ? (ordered?.ThenByDescending(p => p.ProductId) ?? query.OrderByDescending(p => p.ProductId)) : (ordered?.ThenBy(p => p.ProductId) ?? query.OrderBy(p => p.ProductId)),
                    "productcode" => desc ? (ordered?.ThenByDescending(p => p.ProductCode) ?? query.OrderByDescending(p => p.ProductCode)) : (ordered?.ThenBy(p => p.ProductCode) ?? query.OrderBy(p => p.ProductCode)),
                    "productname" => desc ? (ordered?.ThenByDescending(p => p.ProductName) ?? query.OrderByDescending(p => p.ProductName)) : (ordered?.ThenBy(p => p.ProductName) ?? query.OrderBy(p => p.ProductName)),
                    "createdat" => desc ? (ordered?.ThenByDescending(p => p.CreatedAt) ?? query.OrderByDescending(p => p.CreatedAt)) : (ordered?.ThenBy(p => p.CreatedAt) ?? query.OrderBy(p => p.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(p => p.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(p => p.CreatedAt);
        }
    }
}