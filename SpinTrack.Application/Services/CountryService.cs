using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Countries.DTOs;
using SpinTrack.Application.Features.Countries.Interfaces;
using SpinTrack.Application.Features.Countries.Mappers;
using SpinTrack.Core.Entities.Country;

namespace SpinTrack.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateCountryRequest> _createValidator;
        private readonly IValidator<UpdateCountryRequest> _updateValidator;
        private readonly ILogger<CountryService> _logger;

        public CountryService(
            ICountryRepository countryRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateCountryRequest> createValidator,
            IValidator<UpdateCountryRequest> updateValidator,
            ILogger<CountryService> logger)
        {
            _countryRepository = countryRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<CountryDetailDto>> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default)
        {
            var c = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
            if (c == null)
                return Result.Failure<CountryDetailDto>(Error.NotFound("Country", countryId.ToString()));

            return Result.Success(CountryMapper.ToCountryDetailDto(c));
        }

        public async Task<Result<CountryDetailDto>> CreateCountryAsync(CreateCountryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<CountryDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _countryRepository.CountryCodeExistsAsync(request.CountryCodeISO2, cancellationToken: cancellationToken))
            {
                return Result.Failure<CountryDetailDto>(Error.Conflict("Country code already exists"));
            }

            var c = CountryMapper.ToEntity(request);
            await _countryRepository.AddAsync(c, cancellationToken);
            await _countryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Country created: {CountryId} {Name}", c.CountryId, c.CountryName);
            return Result.Success(CountryMapper.ToCountryDetailDto(c));
        }

        public async Task<Result<CountryDetailDto>> UpdateCountryAsync(Guid countryId, UpdateCountryRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<CountryDetailDto>(Error.Validation("Validation failed", errors));
            }

            var c = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
            if (c == null)
                return Result.Failure<CountryDetailDto>(Error.NotFound("Country", countryId.ToString()));

            CountryMapper.UpdateEntity(c, request);
            _countryRepository.Update(c);
            await _countryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Country updated: {CountryId}", countryId);
            return Result.Success(CountryMapper.ToCountryDetailDto(c));
        }

        public async Task<Result> DeleteCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
        {
            var c = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
            if (c == null)
                return Result.Failure(Error.NotFound("Country", countryId.ToString()));

            c.IsDeleted = true;
            _countryRepository.Update(c);
            await _countryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Country soft deleted: {CountryId}", countryId);
            return Result.Success();
        }
    }
}