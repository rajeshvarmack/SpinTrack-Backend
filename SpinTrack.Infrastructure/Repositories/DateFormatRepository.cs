using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.DateFormats.Interfaces;
using SpinTrack.Core.Entities.DateFormat;

namespace SpinTrack.Infrastructure.Repositories
{
    public class DateFormatRepository : IDateFormatRepository
    {
        private readonly SpinTrackDbContext _context;

        public DateFormatRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<DateFormat?> GetByIdAsync(Guid dateFormatId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<DateFormat>().AsNoTracking().FirstOrDefaultAsync(df => df.DateFormatId == dateFormatId, cancellationToken);
        }

        public async Task<DateFormat?> GetByFormatAsync(string formatString, CancellationToken cancellationToken = default)
        {
            return await _context.Set<DateFormat>().AsNoTracking().FirstOrDefaultAsync(df => df.FormatString == formatString, cancellationToken);
        }

        public async Task<bool> FormatExistsAsync(string formatString, Guid? excludeDateFormatId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<DateFormat>().AsNoTracking().Where(df => df.FormatString == formatString);
            if (excludeDateFormatId.HasValue)
                query = query.Where(df => df.DateFormatId != excludeDateFormatId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<DateFormat, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<DateFormat>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(df => df.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<DateFormat, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<DateFormat>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(df => df.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(DateFormat entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<DateFormat>().AddAsync(entity, cancellationToken);
        }

        public void Update(DateFormat entity)
        {
            _context.Set<DateFormat>().Update(entity);
        }

        public void Delete(DateFormat entity)
        {
            _context.Set<DateFormat>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<DateFormat> ApplySorting(IQueryable<DateFormat> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<DateFormat>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "dateformatid" => desc ? (ordered?.ThenByDescending(df => df.DateFormatId) ?? query.OrderByDescending(df => df.DateFormatId)) : (ordered?.ThenBy(df => df.DateFormatId) ?? query.OrderBy(df => df.DateFormatId)),
                    "formatstring" => desc ? (ordered?.ThenByDescending(df => df.FormatString) ?? query.OrderByDescending(df => df.FormatString)) : (ordered?.ThenBy(df => df.FormatString) ?? query.OrderBy(df => df.FormatString)),
                    "createdat" => desc ? (ordered?.ThenByDescending(df => df.CreatedAt) ?? query.OrderByDescending(df => df.CreatedAt)) : (ordered?.ThenBy(df => df.CreatedAt) ?? query.OrderBy(df => df.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(df => df.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(df => df.CreatedAt);
        }
    }
}