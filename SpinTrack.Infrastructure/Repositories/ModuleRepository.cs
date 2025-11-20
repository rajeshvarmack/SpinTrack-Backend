using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Modules.Interfaces;
using SpinTrack.Core.Entities.Module;

namespace SpinTrack.Infrastructure.Repositories
{
    /// <summary>
    /// Module repository implementation using EF Core directly
    /// </summary>
    public class ModuleRepository : IModuleRepository
    {
        private readonly SpinTrackDbContext _context;

        public ModuleRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Module?> GetByIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Module>().AsNoTracking().FirstOrDefaultAsync(m => m.ModuleId == moduleId, cancellationToken);
        }

        public async Task<Module?> GetByKeyAsync(string moduleKey, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Module>().AsNoTracking().FirstOrDefaultAsync(m => m.ModuleKey == moduleKey, cancellationToken);
        }

        public async Task<bool> ModuleKeyExistsAsync(string moduleKey, Guid? excludeModuleId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Module>().AsNoTracking().Where(m => m.ModuleKey == moduleKey);

            if (excludeModuleId.HasValue)
            {
                query = query.Where(m => m.ModuleId != excludeModuleId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Module, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Module>().AsNoTracking();

            if (request.Filters != null && request.Filters.Any())
            {
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortColumns != null && request.SortColumns.Any())
            {
                query = ApplySorting(query, request.SortColumns);
            }
            else
            {
                query = query.OrderByDescending(m => m.CreatedAt);
            }

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mapped = items.Select(mapper).ToList();
            return new PagedResult<TResult>(mapped, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Module, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Module>().AsNoTracking();

            if (request.Filters != null && request.Filters.Any())
            {
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);
            }

            if (request.SortColumns != null && request.SortColumns.Any())
            {
                query = ApplySorting(query, request.SortColumns);
            }
            else
            {
                query = query.OrderByDescending(m => m.CreatedAt);
            }

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(Module module, CancellationToken cancellationToken = default)
        {
            await _context.Set<Module>().AddAsync(module, cancellationToken);
        }

        public void Update(Module module)
        {
            _context.Set<Module>().Update(module);
        }

        public void Delete(Module module)
        {
            _context.Set<Module>().Remove(module);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<Module> ApplySorting(IQueryable<Module> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<Module>? orderedQuery = null;

            foreach (var sortColumn in sortColumns)
            {
                var propertyName = sortColumn.ColumnName;
                var isDescending = sortColumn.Direction == SortDirection.Descending;

                orderedQuery = propertyName.ToLowerInvariant() switch
                {
                    "moduleid" => isDescending
                        ? (orderedQuery?.ThenByDescending(m => m.ModuleId) ?? query.OrderByDescending(m => m.ModuleId))
                        : (orderedQuery?.ThenBy(m => m.ModuleId) ?? query.OrderBy(m => m.ModuleId)),
                    "modulekey" => isDescending
                        ? (orderedQuery?.ThenByDescending(m => m.ModuleKey) ?? query.OrderByDescending(m => m.ModuleKey))
                        : (orderedQuery?.ThenBy(m => m.ModuleKey) ?? query.OrderBy(m => m.ModuleKey)),
                    "modulename" => isDescending
                        ? (orderedQuery?.ThenByDescending(m => m.ModuleName) ?? query.OrderByDescending(m => m.ModuleName))
                        : (orderedQuery?.ThenBy(m => m.ModuleName) ?? query.OrderBy(m => m.ModuleName)),
                    "status" => isDescending
                        ? (orderedQuery?.ThenByDescending(m => m.Status) ?? query.OrderByDescending(m => m.Status))
                        : (orderedQuery?.ThenBy(m => m.Status) ?? query.OrderBy(m => m.Status)),
                    "createdat" => isDescending
                        ? (orderedQuery?.ThenByDescending(m => m.CreatedAt) ?? query.OrderByDescending(m => m.CreatedAt))
                        : (orderedQuery?.ThenBy(m => m.CreatedAt) ?? query.OrderBy(m => m.CreatedAt)),
                    _ => orderedQuery ?? query.OrderByDescending(m => m.CreatedAt)
                };
            }

            return orderedQuery ?? query.OrderByDescending(m => m.CreatedAt);
        }
    }
}