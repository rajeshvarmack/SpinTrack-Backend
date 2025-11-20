using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Currency;

namespace SpinTrack.Application.Features.Currencies.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetByIdAsync(Guid currencyId, CancellationToken cancellationToken = default);
        Task<Currency?> GetByCodeAsync(string currencyCode, CancellationToken cancellationToken = default);
        Task<bool> CurrencyCodeExistsAsync(string currencyCode, Guid? excludeCurrencyId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Currency, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Currency, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(Currency currency, CancellationToken cancellationToken = default);
        void Update(Currency currency);
        void Delete(Currency currency);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}