using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Countries.Interfaces;
using SpinTrack.Core.Entities.Country;

namespace SpinTrack.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly SpinTrackDbContext _context;

        public CountryRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Country?> GetByIdAsync(Guid countryId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Country>().AsNoTracking().FirstOrDefaultAsync(c => c.CountryId == countryId, cancellationToken);
        }

        public async Task<Country?> GetByCodeAsync(string iso2, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Country>().AsNoTracking().FirstOrDefaultAsync(c => c.CountryCodeISO2 == iso2, cancellationToken);
        }

        public async Task<bool> CountryCodeExistsAsync(string iso2, Guid? excludeCountryId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Country>().AsNoTracking().Where(c => c.CountryCodeISO2 == iso2);
            if (excludeCountryId.HasValue)
                query = query.Where(c => c.CountryId != excludeCountryId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Country, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Country>().AsNoTracking();
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

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Country, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Country>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(c => c.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(Country country, CancellationToken cancellationToken = default)
        {
            await _context.Set<Country>().AddAsync(country, cancellationToken);
        }

        public void Update(Country country)
        {
            _context.Set<Country>().Update(country);
        }

        public void Delete(Country country)
        {
            _context.Set<Country>().Remove(country);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<Country> ApplySorting(IQueryable<Country> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<Country>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "countryid" => desc ? (ordered?.ThenByDescending(c => c.CountryId) ?? query.OrderByDescending(c => c.CountryId)) : (ordered?.ThenBy(c => c.CountryId) ?? query.OrderBy(c => c.CountryId)),
                    "countrycodeiso2" => desc ? (ordered?.ThenByDescending(c => c.CountryCodeISO2) ?? query.OrderByDescending(c => c.CountryCodeISO2)) : (ordered?.ThenBy(c => c.CountryCodeISO2) ?? query.OrderBy(c => c.CountryCodeISO2)),
                    "countryname" => desc ? (ordered?.ThenByDescending(c => c.CountryName) ?? query.OrderByDescending(c => c.CountryName)) : (ordered?.ThenBy(c => c.CountryName) ?? query.OrderBy(c => c.CountryName)),
                    "createdat" => desc ? (ordered?.ThenByDescending(c => c.CreatedAt) ?? query.OrderByDescending(c => c.CreatedAt)) : (ordered?.ThenBy(c => c.CreatedAt) ?? query.OrderBy(c => c.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(c => c.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(c => c.CreatedAt);
        }
    }
}