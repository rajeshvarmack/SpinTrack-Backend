using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Permissions.Interfaces;
using SpinTrack.Core.Entities.Permission;

namespace SpinTrack.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly SpinTrackDbContext _context;

        public PermissionRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Permission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Permission>().AsNoTracking().FirstOrDefaultAsync(p => p.PermissionId == permissionId, cancellationToken);
        }

        public async Task<Permission?> GetByKeyAsync(string permissionKey, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Permission>().AsNoTracking().FirstOrDefaultAsync(p => p.PermissionKey == permissionKey, cancellationToken);
        }

        public async Task<bool> PermissionKeyExistsAsync(string permissionKey, Guid? excludePermissionId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Permission>().AsNoTracking().Where(p => p.PermissionKey == permissionKey);
            if (excludePermissionId.HasValue)
                query = query.Where(p => p.PermissionId != excludePermissionId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Permission, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Permission>().AsNoTracking();
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

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Permission, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Permission>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(p => p.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
        {
            await _context.Set<Permission>().AddAsync(permission, cancellationToken);
        }

        public void Update(Permission permission)
        {
            _context.Set<Permission>().Update(permission);
        }

        public void Delete(Permission permission)
        {
            _context.Set<Permission>().Remove(permission);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<Permission> ApplySorting(IQueryable<Permission> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<Permission>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "permissionid" => desc ? (ordered?.ThenByDescending(p => p.PermissionId) ?? query.OrderByDescending(p => p.PermissionId)) : (ordered?.ThenBy(p => p.PermissionId) ?? query.OrderBy(p => p.PermissionId)),
                    "submoduleid" => desc ? (ordered?.ThenByDescending(p => p.SubModuleId) ?? query.OrderByDescending(p => p.SubModuleId)) : (ordered?.ThenBy(p => p.SubModuleId) ?? query.OrderBy(p => p.SubModuleId)),
                    "permissionkey" => desc ? (ordered?.ThenByDescending(p => p.PermissionKey) ?? query.OrderByDescending(p => p.PermissionKey)) : (ordered?.ThenBy(p => p.PermissionKey) ?? query.OrderBy(p => p.PermissionKey)),
                    "permissionname" => desc ? (ordered?.ThenByDescending(p => p.PermissionName) ?? query.OrderByDescending(p => p.PermissionName)) : (ordered?.ThenBy(p => p.PermissionName) ?? query.OrderBy(p => p.PermissionName)),
                    "status" => desc ? (ordered?.ThenByDescending(p => p.Status) ?? query.OrderByDescending(p => p.Status)) : (ordered?.ThenBy(p => p.Status) ?? query.OrderBy(p => p.Status)),
                    "createdat" => desc ? (ordered?.ThenByDescending(p => p.CreatedAt) ?? query.OrderByDescending(p => p.CreatedAt)) : (ordered?.ThenBy(p => p.CreatedAt) ?? query.OrderBy(p => p.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(p => p.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(p => p.CreatedAt);
        }
    }
}