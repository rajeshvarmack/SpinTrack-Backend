using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.BusinessHours.Interfaces;
using SpinTrack.Core.Entities.BusinessHours;

namespace SpinTrack.Infrastructure.Repositories
{
    public class BusinessHoursRepository : IBusinessHoursRepository
    {
        private readonly SpinTrackDbContext _context;

        public BusinessHoursRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessHour?> GetByIdAsync(Guid businessHoursId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BusinessHour>().AsNoTracking().FirstOrDefaultAsync(bh => bh.BusinessHoursId == businessHoursId, cancellationToken);
        }

        public async Task<BusinessHour?> GetByCompanyDayShiftAsync(Guid companyId, string dayOfWeek, string shiftName, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BusinessHour>().AsNoTracking().FirstOrDefaultAsync(bh => bh.CompanyId == companyId && bh.DayOfWeek == dayOfWeek && bh.ShiftName == shiftName, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid companyId, string dayOfWeek, string shiftName, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessHour>().AsNoTracking().Where(bh => bh.CompanyId == companyId && bh.DayOfWeek == dayOfWeek && bh.ShiftName == shiftName);
            if (excludeId.HasValue)
                query = query.Where(bh => bh.BusinessHoursId != excludeId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<BusinessHour, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessHour>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(bh => bh.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<BusinessHour, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessHour>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(bh => bh.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(BusinessHour entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<BusinessHour>().AddAsync(entity, cancellationToken);
        }

        public void Update(BusinessHour entity)
        {
            _context.Set<BusinessHour>().Update(entity);
        }

        public void Delete(BusinessHour entity)
        {
            _context.Set<BusinessHour>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<BusinessHour> ApplySorting(IQueryable<BusinessHour> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<BusinessHour>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "businesshoursid" => desc ? (ordered?.ThenByDescending(bh => bh.BusinessHoursId) ?? query.OrderByDescending(bh => bh.BusinessHoursId)) : (ordered?.ThenBy(bh => bh.BusinessHoursId) ?? query.OrderBy(bh => bh.BusinessHoursId)),
                    "dayofweek" => desc ? (ordered?.ThenByDescending(bh => bh.DayOfWeek) ?? query.OrderByDescending(bh => bh.DayOfWeek)) : (ordered?.ThenBy(bh => bh.DayOfWeek) ?? query.OrderBy(bh => bh.DayOfWeek)),
                    "createdat" => desc ? (ordered?.ThenByDescending(bh => bh.CreatedAt) ?? query.OrderByDescending(bh => bh.CreatedAt)) : (ordered?.ThenBy(bh => bh.CreatedAt) ?? query.OrderBy(bh => bh.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(bh => bh.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(bh => bh.CreatedAt);
        }
    }
}