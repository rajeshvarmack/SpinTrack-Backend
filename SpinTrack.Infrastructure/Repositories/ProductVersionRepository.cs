using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.ProductVersions.Interfaces;
using SpinTrack.Core.Entities.ProductVersion;

namespace SpinTrack.Infrastructure.Repositories
{
    public class ProductVersionRepository : IProductVersionRepository
    {
        private readonly SpinTrackDbContext _context;

        public ProductVersionRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVersion?> GetByIdAsync(Guid productVersionId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ProductVersion>().AsNoTracking().FirstOrDefaultAsync(pv => pv.ProductVersionId == productVersionId, cancellationToken);
        }

        public async Task<ProductVersion?> GetByProductAndVersionAsync(Guid productId, string versionNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ProductVersion>().AsNoTracking().FirstOrDefaultAsync(pv => pv.ProductId == productId && pv.VersionNumber == versionNumber, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid productId, string versionNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ProductVersion>().AsNoTracking().Where(pv => pv.ProductId == productId && pv.VersionNumber == versionNumber);
            if (excludeId.HasValue)
                query = query.Where(pv => pv.ProductVersionId != excludeId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<ProductVersion, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ProductVersion>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(pv => pv.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<ProductVersion, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ProductVersion>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(pv => pv.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(ProductVersion entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<ProductVersion>().AddAsync(entity, cancellationToken);
        }

        public void Update(ProductVersion entity)
        {
            _context.Set<ProductVersion>().Update(entity);
        }

        public void Delete(ProductVersion entity)
        {
            _context.Set<ProductVersion>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<ProductVersion> ApplySorting(IQueryable<ProductVersion> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<ProductVersion>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "productversionid" => desc ? (ordered?.ThenByDescending(pv => pv.ProductVersionId) ?? query.OrderByDescending(pv => pv.ProductVersionId)) : (ordered?.ThenBy(pv => pv.ProductVersionId) ?? query.OrderBy(pv => pv.ProductVersionId)),
                    "versionnumber" => desc ? (ordered?.ThenByDescending(pv => pv.VersionNumber) ?? query.OrderByDescending(pv => pv.VersionNumber)) : (ordered?.ThenBy(pv => pv.VersionNumber) ?? query.OrderBy(pv => pv.VersionNumber)),
                    "createdat" => desc ? (ordered?.ThenByDescending(pv => pv.CreatedAt) ?? query.OrderByDescending(pv => pv.CreatedAt)) : (ordered?.ThenBy(pv => pv.CreatedAt) ?? query.OrderBy(pv => pv.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(pv => pv.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(pv => pv.CreatedAt);
        }
    }
}