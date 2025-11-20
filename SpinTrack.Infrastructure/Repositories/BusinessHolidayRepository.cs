using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.BusinessHolidays.Interfaces;
using SpinTrack.Core.Entities.BusinessHoliday;

namespace SpinTrack.Infrastructure.Repositories
{
    public class BusinessHolidayRepository : IBusinessHolidayRepository
    {
        private readonly SpinTrackDbContext _context;

        public BusinessHolidayRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessHoliday?> GetByIdAsync(Guid businessHolidayId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BusinessHoliday>().AsNoTracking().FirstOrDefaultAsync(bh => bh.BusinessHolidayId == businessHolidayId, cancellationToken);
        }

        public async Task<BusinessHoliday?> GetByCompanyAndDateAsync(Guid companyId, DateOnly holidayDate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BusinessHoliday>().AsNoTracking().FirstOrDefaultAsync(bh => bh.CompanyId == companyId && bh.HolidayDate == holidayDate, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid companyId, DateOnly holidayDate, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessHoliday>().AsNoTracking().Where(bh => bh.CompanyId == companyId && bh.HolidayDate == holidayDate);
            if (excludeId.HasValue)
                query = query.Where(bh => bh.BusinessHolidayId != excludeId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<BusinessHoliday, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessHoliday>().AsNoTracking();
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

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<BusinessHoliday, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessHoliday>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(bh => bh.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(BusinessHoliday entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<BusinessHoliday>().AddAsync(entity, cancellationToken);
        }

        public void Update(BusinessHoliday entity)
        {
            _context.Set<BusinessHoliday>().Update(entity);
        }

        public void Delete(BusinessHoliday entity)
        {
            _context.Set<BusinessHoliday>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<BusinessHoliday> ApplySorting(IQueryable<BusinessHoliday> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<BusinessHoliday>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "businessholidayid" => desc ? (ordered?.ThenByDescending(bh => bh.BusinessHolidayId) ?? query.OrderByDescending(bh => bh.BusinessHolidayId)) : (ordered?.ThenBy(bh => bh.BusinessHolidayId) ?? query.OrderBy(bh => bh.BusinessHolidayId)),
                    "holidaydate" => desc ? (ordered?.ThenByDescending(bh => bh.HolidayDate) ?? query.OrderByDescending(bh => bh.HolidayDate)) : (ordered?.ThenBy(bh => bh.HolidayDate) ?? query.OrderBy(bh => bh.HolidayDate)),
                    "createdat" => desc ? (ordered?.ThenByDescending(bh => bh.CreatedAt) ?? query.OrderByDescending(bh => bh.CreatedAt)) : (ordered?.ThenBy(bh => bh.CreatedAt) ?? query.OrderBy(bh => bh.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(bh => bh.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(bh => bh.CreatedAt);
        }
    }
}