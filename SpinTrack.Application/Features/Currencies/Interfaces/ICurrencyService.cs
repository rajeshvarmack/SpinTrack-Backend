using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Currencies.DTOs;

namespace SpinTrack.Application.Features.Currencies.Interfaces
{
    public interface ICurrencyService
    {
        Task<Result<CurrencyDetailDto>> GetCurrencyByIdAsync(Guid currencyId, CancellationToken cancellationToken = default);
        Task<Result<CurrencyDetailDto>> CreateCurrencyAsync(CreateCurrencyRequest request, CancellationToken cancellationToken = default);
        Task<Result<CurrencyDetailDto>> UpdateCurrencyAsync(Guid currencyId, UpdateCurrencyRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    }
}