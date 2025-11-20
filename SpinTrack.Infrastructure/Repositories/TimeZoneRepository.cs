using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.TimeZones.Interfaces;
using SpinTrack.Core.Entities.TimeZone;

namespace SpinTrack.Infrastructure.Repositories
{
    public class TimeZoneRepository : ITimeZoneRepository
    {
        private readonly SpinTrackDbContext _context;

        public TimeZoneRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<TimeZoneEntity?> GetByIdAsync(Guid timeZoneId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TimeZoneEntity>().AsNoTracking().FirstOrDefaultAsync(tz => tz.TimeZoneId == timeZoneId, cancellationToken);
        }

        public async Task<TimeZoneEntity?> GetByNameAsync(string timeZoneName, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TimeZoneEntity>().AsNoTracking().FirstOrDefaultAsync(tz => tz.TimeZoneName == timeZoneName, cancellationToken);
        }

        public async Task<bool> TimeZoneNameExistsAsync(string timeZoneName, Guid? excludeTimeZoneId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TimeZoneEntity>().AsNoTracking().Where(tz => tz.TimeZoneName == timeZoneName);
            if (excludeTimeZoneId.HasValue)
                query = query.Where(tz => tz.TimeZoneId != excludeTimeZoneId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<TimeZoneEntity, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TimeZoneEntity>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(tz => tz.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<TimeZoneEntity, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TimeZoneEntity>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(tz => tz.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(TimeZoneEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<TimeZoneEntity>().AddAsync(entity, cancellationToken);
        }

        public void Update(TimeZoneEntity entity)
        {
            _context.Set<TimeZoneEntity>().Update(entity);
        }

        public void Delete(TimeZoneEntity entity)
        {
            _context.Set<TimeZoneEntity>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<TimeZoneEntity> ApplySorting(IQueryable<TimeZoneEntity> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<TimeZoneEntity>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "timezoneid" => desc ? (ordered?.ThenByDescending(tz => tz.TimeZoneId) ?? query.OrderByDescending(tz => tz.TimeZoneId)) : (ordered?.ThenBy(tz => tz.TimeZoneId) ?? query.OrderBy(tz => tz.TimeZoneId)),
                    "timezonename" => desc ? (ordered?.ThenByDescending(tz => tz.TimeZoneName) ?? query.OrderByDescending(tz => tz.TimeZoneName)) : (ordered?.ThenBy(tz => tz.TimeZoneName) ?? query.OrderBy(tz => tz.TimeZoneName)),
                    "createdat" => desc ? (ordered?.ThenByDescending(tz => tz.CreatedAt) ?? query.OrderByDescending(tz => tz.CreatedAt)) : (ordered?.ThenBy(tz => tz.CreatedAt) ?? query.OrderBy(tz => tz.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(tz => tz.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(tz => tz.CreatedAt);
        }
    }
}