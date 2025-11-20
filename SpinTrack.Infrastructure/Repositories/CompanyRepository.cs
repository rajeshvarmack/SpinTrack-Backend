using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Common.Helpers;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Features.Companies.Interfaces;
using SpinTrack.Core.Entities.Company;

namespace SpinTrack.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly SpinTrackDbContext _context;

        public CompanyRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Company>().AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == companyId, cancellationToken);
        }

        public async Task<Company?> GetByCodeAsync(string companyCode, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Company>().AsNoTracking().FirstOrDefaultAsync(c => c.CompanyCode == companyCode, cancellationToken);
        }

        public async Task<bool> CompanyCodeExistsAsync(string companyCode, Guid? excludeCompanyId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Company>().AsNoTracking().Where(c => c.CompanyCode == companyCode);
            if (excludeCompanyId.HasValue)
                query = query.Where(c => c.CompanyId != excludeCompanyId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Company, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Company>().AsNoTracking();
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

        public async Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Company, TResult> mapper, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Company>().AsNoTracking();
            if (request.Filters != null && request.Filters.Any())
                query = FilterExpressionBuilder.ApplyFilters(query, request.Filters);

            if (request.SortColumns != null && request.SortColumns.Any())
                query = ApplySorting(query, request.SortColumns);
            else
                query = query.OrderByDescending(c => c.CreatedAt);

            var items = await query.ToListAsync(cancellationToken);
            return items.Select(mapper).ToList();
        }

        public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
        {
            await _context.Set<Company>().AddAsync(company, cancellationToken);
        }

        public void Update(Company company)
        {
            _context.Set<Company>().Update(company);
        }

        public void Delete(Company company)
        {
            _context.Set<Company>().Remove(company);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private static IQueryable<Company> ApplySorting(IQueryable<Company> query, List<SortColumn> sortColumns)
        {
            IOrderedQueryable<Company>? ordered = null;
            foreach (var sort in sortColumns)
            {
                var prop = sort.ColumnName.ToLowerInvariant();
                var desc = sort.Direction == SortDirection.Descending;
                ordered = prop switch
                {
                    "companyid" => desc ? (ordered?.ThenByDescending(c => c.CompanyId) ?? query.OrderByDescending(c => c.CompanyId)) : (ordered?.ThenBy(c => c.CompanyId) ?? query.OrderBy(c => c.CompanyId)),
                    "companycode" => desc ? (ordered?.ThenByDescending(c => c.CompanyCode) ?? query.OrderByDescending(c => c.CompanyCode)) : (ordered?.ThenBy(c => c.CompanyCode) ?? query.OrderBy(c => c.CompanyCode)),
                    "companyname" => desc ? (ordered?.ThenByDescending(c => c.CompanyName) ?? query.OrderByDescending(c => c.CompanyName)) : (ordered?.ThenBy(c => c.CompanyName) ?? query.OrderBy(c => c.CompanyName)),
                    "createdat" => desc ? (ordered?.ThenByDescending(c => c.CreatedAt) ?? query.OrderByDescending(c => c.CreatedAt)) : (ordered?.ThenBy(c => c.CreatedAt) ?? query.OrderBy(c => c.CreatedAt)),
                    _ => ordered ?? query.OrderByDescending(c => c.CreatedAt)
                };
            }

            return ordered ?? query.OrderByDescending(c => c.CreatedAt);
        }
    }
}