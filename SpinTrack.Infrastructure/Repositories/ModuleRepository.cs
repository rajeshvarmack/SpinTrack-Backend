using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Modules.Interfaces;
using SpinTrack.Core.Entities.Module;

namespace SpinTrack.Infrastructure.Repositories
{
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
                query = query.Where(m => m.ModuleId != excludeModuleId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Module, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Module>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(m => m.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Module, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Module>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(m => m.CreatedAt);

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
            IOrderedQueryable<Module>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "moduleid" => desc ? (ordered?.ThenByDescending(m => m.ModuleId) ?? query.OrderByDescending(m => m.ModuleId)) : (ordered?.ThenBy(m => m.ModuleId) ?? query.OrderBy(m => m.ModuleId)),
                    "modulekey" => desc ? (ordered?.ThenByDescending(m => m.ModuleKey) ?? query.OrderByDescending(m => m.ModuleKey)) : (ordered?.ThenBy(m => m.ModuleKey) ?? query.OrderBy(m => m.ModuleKey)),
                    "modulename" => desc ? (ordered?.ThenByDescending(m => m.ModuleName) ?? query.OrderByDescending(m => m.ModuleName)) : (ordered?.ThenBy(m => m.ModuleName) ?? query.OrderBy(m => m.ModuleName)),
                    "status" => desc ? (ordered?.ThenByDescending(m => m.Status) ?? query.OrderByDescending(m => m.Status)) : (ordered?.ThenBy(m => m.Status) ?? query.OrderBy(m => m.Status)),
                    "createdat" => desc ? (ordered?.ThenByDescending(m => m.CreatedAt) ?? query.OrderByDescending(m => m.CreatedAt)) : (ordered?.ThenBy(m => m.CreatedAt) ?? query.OrderBy(m => m.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(m => m.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(m => m.CreatedAt);
        }
    }
}