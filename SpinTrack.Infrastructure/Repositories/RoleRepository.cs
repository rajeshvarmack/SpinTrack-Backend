using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Roles.Interfaces;
using SpinTrack.Core.Entities.Role;

namespace SpinTrack.Infrastructure.Repositories
{
    /// <summary>
    /// Role repository implementation using EF Core directly
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly SpinTrackDbContext _context;

        public RoleRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Role>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleId == roleId, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Role>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken);
        }

        public async Task<bool> RoleNameExistsAsync(string roleName, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Role>().AsNoTracking().Where(r => r.RoleName == roleName);

            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.RoleId != excludeRoleId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(
            QueryRequest request,
            Func<Role, TResult> mapper,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Role>().AsNoTracking();

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
                query = query.OrderByDescending(r => r.CreatedAt);
            }

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mapped = items.Select(mapper).ToList();
            return new PagedResult<TResult>(mapped, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(
            QueryRequest request,
            Func<Role, TResult> mapper,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Role>().AsNoTracking();

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
                query = query.OrderByDescending(r => r.CreatedAt);
            }

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            await _context.Set<Role>().AddAsync(role, cancellationToken);
        }

        public void Update(Role role)
        {
            _context.Set<Role>().Update(role);
        }

        public void Delete(Role role)
        {
            _context.Set<Role>().Remove(role);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<Role> ApplySorting(IQueryable<Role> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<Role>? orderedQuery = null;

            foreach (var sortColumn in sortColumns)
            {
                var propertyName = sortColumn.ColumnName;
                var isDescending = sortColumn.Direction == SortDirection.Descending;

                orderedQuery = propertyName.ToLowerInvariant() switch
                {
                    "roleid" => isDescending
                        ? (orderedQuery?.ThenByDescending(r => r.RoleId) ?? query.OrderByDescending(r => r.RoleId))
                        : (orderedQuery?.ThenBy(r => r.RoleId) ?? query.OrderBy(r => r.RoleId)),
                    "rolename" => isDescending
                        ? (orderedQuery?.ThenByDescending(r => r.RoleName) ?? query.OrderByDescending(r => r.RoleName))
                        : (orderedQuery?.ThenBy(r => r.RoleName) ?? query.OrderBy(r => r.RoleName)),
                    "status" => isDescending
                        ? (orderedQuery?.ThenByDescending(r => r.Status) ?? query.OrderByDescending(r => r.Status))
                        : (orderedQuery?.ThenBy(r => r.Status) ?? query.OrderBy(r => r.Status)),
                    "createdat" => isDescending
                        ? (orderedQuery?.ThenByDescending(r => r.CreatedAt) ?? query.OrderByDescending(r => r.CreatedAt))
                        : (orderedQuery?.ThenBy(r => r.CreatedAt) ?? query.OrderBy(r => r.CreatedAt)),
                    _ => orderedQuery ?? query.OrderByDescending(r => r.CreatedAt)
                };
            }

            return orderedQuery ?? query.OrderByDescending(r => r.CreatedAt);
        }
    }
}