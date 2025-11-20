using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.BusinessDays.Interfaces;
using SpinTrack.Core.Entities.BusinessDay;

namespace SpinTrack.Infrastructure.Repositories
{
    public class BusinessDayRepository : IBusinessDayRepository
    {
        private readonly SpinTrackDbContext _context;

        public BusinessDayRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessDay?> GetByIdAsync(Guid businessDayId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BusinessDay>().AsNoTracking().FirstOrDefaultAsync(bd => bd.BusinessDayId == businessDayId, cancellationToken);
        }

        public async Task<BusinessDay?> GetByCompanyAndDayAsync(Guid companyId, string dayOfWeek, CancellationToken cancellationToken = default)
        {
            return await _context.Set<BusinessDay>().AsNoTracking().FirstOrDefaultAsync(bd => bd.CompanyId == companyId && bd.DayOfWeek == dayOfWeek, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid companyId, string dayOfWeek, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessDay>().AsNoTracking().Where(bd => bd.CompanyId == companyId && bd.DayOfWeek == dayOfWeek);
            if (excludeId.HasValue)
                query = query.Where(bd => bd.BusinessDayId != excludeId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<BusinessDay, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessDay>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(bd => bd.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<BusinessDay, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<BusinessDay>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(bd => bd.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(BusinessDay entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<BusinessDay>().AddAsync(entity, cancellationToken);
        }

        public void Update(BusinessDay entity)
        {
            _context.Set<BusinessDay>().Update(entity);
        }

        public void Delete(BusinessDay entity)
        {
            _context.Set<BusinessDay>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<BusinessDay> ApplySorting(IQueryable<BusinessDay> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<BusinessDay>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "businessdayid" => desc ? (ordered?.ThenByDescending(bd => bd.BusinessDayId) ?? query.OrderByDescending(bd => bd.BusinessDayId)) : (ordered?.ThenBy(bd => bd.BusinessDayId) ?? query.OrderBy(bd => bd.BusinessDayId)),
                    "dayofweek" => desc ? (ordered?.ThenByDescending(bd => bd.DayOfWeek) ?? query.OrderByDescending(bd => bd.DayOfWeek)) : (ordered?.ThenBy(bd => bd.DayOfWeek) ?? query.OrderBy(bd => bd.DayOfWeek)),
                    "createdat" => desc ? (ordered?.ThenByDescending(bd => bd.CreatedAt) ?? query.OrderByDescending(bd => bd.CreatedAt)) : (ordered?.ThenBy(bd => bd.CreatedAt) ?? query.OrderBy(bd => bd.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(bd => bd.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(bd => bd.CreatedAt);
        }
    }
}