using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.SubModules.Interfaces;
using SpinTrack.Core.Entities.SubModule;

namespace SpinTrack.Infrastructure.Repositories
{
    public class SubModuleRepository : ISubModuleRepository
    {
        private readonly SpinTrackDbContext _context;

        public SubModuleRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<SubModule?> GetByIdAsync(Guid subModuleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<SubModule>().AsNoTracking().FirstOrDefaultAsync(s => s.SubModuleId == subModuleId, cancellationToken);
        }

        public async Task<SubModule?> GetByKeyAsync(string subModuleKey, CancellationToken cancellationToken = default)
        {
            return await _context.Set<SubModule>().AsNoTracking().FirstOrDefaultAsync(s => s.SubModuleKey == subModuleKey, cancellationToken);
        }

        public async Task<bool> SubModuleKeyExistsAsync(string subModuleKey, Guid? excludeSubModuleId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<SubModule>().AsNoTracking().Where(s => s.SubModuleKey == subModuleKey);
            if (excludeSubModuleId.HasValue)
                query = query.Where(s => s.SubModuleId != excludeSubModuleId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<SubModule, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<SubModule>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(s => s.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<SubModule, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<SubModule>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(s => s.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(SubModule subModule, CancellationToken cancellationToken = default)
        {
            await _context.Set<SubModule>().AddAsync(subModule, cancellationToken);
        }

        public void Update(SubModule subModule)
        {
            _context.Set<SubModule>().Update(subModule);
        }

        public void Delete(SubModule subModule)
        {
            _context.Set<SubModule>().Remove(subModule);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<SubModule> ApplySorting(IQueryable<SubModule> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<SubModule>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "submoduleid" => desc ? (ordered?.ThenByDescending(s => s.SubModuleId) ?? query.OrderByDescending(s => s.SubModuleId)) : (ordered?.ThenBy(s => s.SubModuleId) ?? query.OrderBy(s => s.SubModuleId)),
                    "moduleid" => desc ? (ordered?.ThenByDescending(s => s.ModuleId) ?? query.OrderByDescending(s => s.ModuleId)) : (ordered?.ThenBy(s => s.ModuleId) ?? query.OrderBy(s => s.ModuleId)),
                    "submodulekey" => desc ? (ordered?.ThenByDescending(s => s.SubModuleKey) ?? query.OrderByDescending(s => s.SubModuleKey)) : (ordered?.ThenBy(s => s.SubModuleKey) ?? query.OrderBy(s => s.SubModuleKey)),
                    "submodulename" => desc ? (ordered?.ThenByDescending(s => s.SubModuleName) ?? query.OrderByDescending(s => s.SubModuleName)) : (ordered?.ThenBy(s => s.SubModuleName) ?? query.OrderBy(s => s.SubModuleName)),
                    "status" => desc ? (ordered?.ThenByDescending(s => s.Status) ?? query.OrderByDescending(s => s.Status)) : (ordered?.ThenBy(s => s.Status) ?? query.OrderBy(s => s.Status)),
                    "createdat" => desc ? (ordered?.ThenByDescending(s => s.CreatedAt) ?? query.OrderByDescending(s => s.CreatedAt)) : (ordered?.ThenBy(s => s.CreatedAt) ?? query.OrderBy(s => s.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(s => s.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(s => s.CreatedAt);
        }
    }
}