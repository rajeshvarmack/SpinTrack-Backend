using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Users.Interfaces;
using SpinTrack.Core.Entities.Auth;

namespace SpinTrack.Infrastructure.Repositories
{
    /// <summary>
    /// User repository implementation using EF Core directly
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly SpinTrackDbContext _context;

        public UserRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        // Read operations with AsNoTracking for better performance
        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Users.AsNoTracking().Where(u => u.Username == username);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserId != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Users.AsNoTracking().Where(u => u.Email == email);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserId != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(
            QueryRequest request,
            Func<User, TResult> mapper,
            CancellationToken cancellationToken = default)
        {
            // Start with base query - AsNoTracking for read-only operations
            var query = _context.Users.AsNoTracking();

            // Apply filters using FilterExpressionBuilder
            if (request.Filters != null && request.Filters.Any())
            {
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            if (request.SortColumns != null && request.SortColumns.Any())
            {
                query = ApplySorting(query, request.SortColumns);
            }
            else
            {
                // Default sorting by CreatedAt descending
                query = query.OrderByDescending(u => u.CreatedAt);
            }

            // Apply pagination
            var users = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Map to result type
            var items = users.Select(mapper).ToList();

            return new PagedResult<TResult>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(
            QueryRequest request,
            Func<User, TResult> mapper,
            CancellationToken cancellationToken = default)
        {
            // Start with base query - AsNoTracking for read-only operations
            var query = _context.Users.AsNoTracking();

            // Apply filters
            if (request.Filters != null && request.Filters.Any())
            {
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);
            }

            // Apply sorting
            if (request.SortColumns != null && request.SortColumns.Any())
            {
                query = ApplySorting(query, request.SortColumns);
            }
            else
            {
                // Default sorting by CreatedAt descending
                query = query.OrderByDescending(u => u.CreatedAt);
            }

            // Get all users (no pagination)
            var users = await query.ToListAsync(cancellationToken);

            // Map to result type
            return users.Select(mapper).ToList();
        }

        // Write operations (with change tracking)
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // Helper method for sorting
        private static IQueryable<User> ApplySorting(IQueryable<User> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<User>? orderedQuery = null;

            foreach (var sortColumn in sortColumns)
            {
                var propertyName = sortColumn.ColumnName;
                var isDescending = sortColumn.Direction == SortDirection.Descending;

                // Apply sorting based on property name
                orderedQuery = propertyName.ToLowerInvariant() switch
                {
                    "userid" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.UserId) ?? query.OrderByDescending(u => u.UserId))
                        : (orderedQuery?.ThenBy(u => u.UserId) ?? query.OrderBy(u => u.UserId)),
                    "username" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.Username) ?? query.OrderByDescending(u => u.Username))
                        : (orderedQuery?.ThenBy(u => u.Username) ?? query.OrderBy(u => u.Username)),
                    "email" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.Email) ?? query.OrderByDescending(u => u.Email))
                        : (orderedQuery?.ThenBy(u => u.Email) ?? query.OrderBy(u => u.Email)),
                    "firstname" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.FirstName) ?? query.OrderByDescending(u => u.FirstName))
                        : (orderedQuery?.ThenBy(u => u.FirstName) ?? query.OrderBy(u => u.FirstName)),
                    "lastname" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.LastName) ?? query.OrderByDescending(u => u.LastName))
                        : (orderedQuery?.ThenBy(u => u.LastName) ?? query.OrderBy(u => u.LastName)),
                    "dateofbirth" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.DateOfBirth) ?? query.OrderByDescending(u => u.DateOfBirth))
                        : (orderedQuery?.ThenBy(u => u.DateOfBirth) ?? query.OrderBy(u => u.DateOfBirth)),
                    "status" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.Status) ?? query.OrderByDescending(u => u.Status))
                        : (orderedQuery?.ThenBy(u => u.Status) ?? query.OrderBy(u => u.Status)),
                    "createdat" => isDescending
                        ? (orderedQuery?.ThenByDescending(u => u.CreatedAt) ?? query.OrderByDescending(u => u.CreatedAt))
                        : (orderedQuery?.ThenBy(u => u.CreatedAt) ?? query.OrderBy(u => u.CreatedAt)),
                    _ => orderedQuery ?? query.OrderByDescending(u => u.CreatedAt)
                };
            }

            return orderedQuery ?? query.OrderByDescending(u => u.CreatedAt);
        }
    }
}
