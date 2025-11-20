using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Country;

namespace SpinTrack.Application.Features.Countries.Interfaces
{
    public interface ICountryRepository
    {
        Task<Country?> GetByIdAsync(Guid countryId, CancellationToken cancellationToken = default);
        Task<Country?> GetByCodeAsync(string iso2, CancellationToken cancellationToken = default);
        Task<bool> CountryCodeExistsAsync(string iso2, Guid? excludeCountryId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Country, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Country, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(Country country, CancellationToken cancellationToken = default);
        void Update(Country country);
        void Delete(Country country);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}