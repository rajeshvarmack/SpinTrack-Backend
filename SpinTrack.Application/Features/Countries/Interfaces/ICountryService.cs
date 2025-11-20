using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Countries.DTOs;

namespace SpinTrack.Application.Features.Countries.Interfaces
{
    public interface ICountryService
    {
        Task<Result<CountryDetailDto>> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default);
        Task<Result<CountryDetailDto>> CreateCountryAsync(CreateCountryRequest request, CancellationToken cancellationToken = default);
        Task<Result<CountryDetailDto>> UpdateCountryAsync(Guid countryId, UpdateCountryRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteCountryAsync(Guid countryId, CancellationToken cancellationToken = default);
    }
}