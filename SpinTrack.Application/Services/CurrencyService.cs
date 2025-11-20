using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Currencies.DTOs;
using SpinTrack.Application.Features.Currencies.Interfaces;
using SpinTrack.Application.Features.Currencies.Mappers;
using SpinTrack.Core.Entities.Currency;

namespace SpinTrack.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateCurrencyRequest> _createValidator;
        private readonly IValidator<UpdateCurrencyRequest> _updateValidator;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(
            ICurrencyRepository currencyRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateCurrencyRequest> createValidator,
            IValidator<UpdateCurrencyRequest> updateValidator,
            ILogger<CurrencyService> logger)
        {
            _currencyRepository = currencyRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<CurrencyDetailDto>> GetCurrencyByIdAsync(Guid currencyId, CancellationToken cancellationToken = default)
        {
            var c = await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);
            if (c == null)
                return Result.Failure<CurrencyDetailDto>(Error.NotFound("Currency", currencyId.ToString()));

            return Result.Success(CurrencyMapper.ToCurrencyDetailDto(c));
        }

        public async Task<Result<CurrencyDetailDto>> CreateCurrencyAsync(CreateCurrencyRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<CurrencyDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _currencyRepository.CurrencyCodeExistsAsync(request.CurrencyCode, cancellationToken: cancellationToken))
            {
                return Result.Failure<CurrencyDetailDto>(Error.Conflict("Currency code already exists"));
            }

            var c = CurrencyMapper.ToEntity(request);
            await _currencyRepository.AddAsync(c, cancellationToken);
            await _currencyRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Currency created: {CurrencyId} Code: {Code}", c.CurrencyId, c.CurrencyCode);
            return Result.Success(CurrencyMapper.ToCurrencyDetailDto(c));
        }

        public async Task<Result<CurrencyDetailDto>> UpdateCurrencyAsync(Guid currencyId, UpdateCurrencyRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<CurrencyDetailDto>(Error.Validation("Validation failed", errors));
            }

            var c = await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);
            if (c == null)
                return Result.Failure<CurrencyDetailDto>(Error.NotFound("Currency", currencyId.ToString()));

            CurrencyMapper.UpdateEntity(c, request);
            _currencyRepository.Update(c);
            await _currencyRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Currency updated: {CurrencyId}", currencyId);
            return Result.Success(CurrencyMapper.ToCurrencyDetailDto(c));
        }

        public async Task<Result> DeleteCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default)
        {
            var c = await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);
            if (c == null)
                return Result.Failure(Error.NotFound("Currency", currencyId.ToString()));

            c.IsDeleted = true;
            _currencyRepository.Update(c);
            await _currencyRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Currency soft deleted: {CurrencyId}", currencyId);
            return Result.Success();
        }
    }
}