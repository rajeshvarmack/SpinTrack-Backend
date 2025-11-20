using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Currencies.Interfaces;
using SpinTrack.Core.Entities.Currency;

namespace SpinTrack.Infrastructure.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly SpinTrackDbContext _context;

        public CurrencyRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Currency?> GetByIdAsync(Guid currencyId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Currency>().AsNoTracking().FirstOrDefaultAsync(c => c.CurrencyId == currencyId, cancellationToken);
        }

        public async Task<Currency?> GetByCodeAsync(string currencyCode, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Currency>().AsNoTracking().FirstOrDefaultAsync(c => c.CurrencyCode == currencyCode, cancellationToken);
        }

        public async Task<bool> CurrencyCodeExistsAsync(string currencyCode, Guid? excludeCurrencyId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Currency>().AsNoTracking().Where(c => c.CurrencyCode == currencyCode);
            if (excludeCurrencyId.HasValue)
                query = query.Where(c => c.CurrencyId != excludeCurrencyId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Currency, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Currency>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            var total = await query.CountAsync(cancellationToken);
            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(c => c.CreatedAt);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            return new PagedResult<TResult>(items.Select(mapper).ToList(), total, request.PageNumber, request.PageSize);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Currency, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Currency>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(c => c.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            await _context.Set<Currency>().AddAsync(currency, cancellationToken);
        }

        public void Update(Currency currency)
        {
            _context.Set<Currency>().Update(currency);
        }

        public void Delete(Currency currency)
        {
            _context.Set<Currency>().Remove(currency);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<Currency> ApplySorting(IQueryable<Currency> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<Currency>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "currencyid" => desc ? (ordered?.ThenByDescending(c => c.CurrencyId) ?? query.OrderByDescending(c => c.CurrencyId)) : (ordered?.ThenBy(c => c.CurrencyId) ?? query.OrderBy(c => c.CurrencyId)),
                    "currencycode" => desc ? (ordered?.ThenByDescending(c => c.CurrencyCode) ?? query.OrderByDescending(c => c.CurrencyCode)) : (ordered?.ThenBy(c => c.CurrencyCode) ?? query.OrderBy(c => c.CurrencyCode)),
                    "createdat" => desc ? (ordered?.ThenByDescending(c => c.CreatedAt) ?? query.OrderByDescending(c => c.CreatedAt)) : (ordered?.ThenBy(c => c.CreatedAt) ?? query.OrderBy(c => c.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(c => c.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(c => c.CreatedAt);
        }
    }
}