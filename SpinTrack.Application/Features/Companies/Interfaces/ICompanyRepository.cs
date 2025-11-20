using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Company;

namespace SpinTrack.Application.Features.Companies.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<Company?> GetByCodeAsync(string companyCode, CancellationToken cancellationToken = default);
        Task<bool> CompanyCodeExistsAsync(string companyCode, Guid? excludeCompanyId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Company, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Company, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(Company company, CancellationToken cancellationToken = default);
        void Update(Company company);
        void Delete(Company company);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}