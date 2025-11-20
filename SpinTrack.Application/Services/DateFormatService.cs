using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.DateFormats.DTOs;
using SpinTrack.Application.Features.DateFormats.Interfaces;
using SpinTrack.Application.Features.DateFormats.Mappers;
using SpinTrack.Core.Entities.DateFormat;

namespace SpinTrack.Application.Services
{
    public class DateFormatService : IDateFormatService
    {
        private readonly IDateFormatRepository _dateFormatRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateDateFormatRequest> _createValidator;
        private readonly IValidator<UpdateDateFormatRequest> _updateValidator;
        private readonly ILogger<DateFormatService> _logger;

        public DateFormatService(
            IDateFormatRepository dateFormatRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateDateFormatRequest> createValidator,
            IValidator<UpdateDateFormatRequest> updateValidator,
            ILogger<DateFormatService> logger)
        {
            _dateFormatRepository = dateFormatRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<DateFormatDetailDto>> GetDateFormatByIdAsync(Guid dateFormatId, CancellationToken cancellationToken = default)
        {
            var df = await _dateFormatRepository.GetByIdAsync(dateFormatId, cancellationToken);
            if (df == null)
                return Result.Failure<DateFormatDetailDto>(Error.NotFound("DateFormat", dateFormatId.ToString()));

            return Result.Success(DateFormatMapper.ToDateFormatDetailDto(df));
        }

        public async Task<Result<DateFormatDetailDto>> CreateDateFormatAsync(CreateDateFormatRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<DateFormatDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _dateFormatRepository.FormatExistsAsync(request.FormatString, cancellationToken: cancellationToken))
            {
                return Result.Failure<DateFormatDetailDto>(Error.Conflict("Date format already exists"));
            }

            var df = DateFormatMapper.ToEntity(request);
            await _dateFormatRepository.AddAsync(df, cancellationToken);
            await _dateFormatRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("DateFormat created: {DateFormatId} Format: {Format}", df.DateFormatId, df.FormatString);
            return Result.Success(DateFormatMapper.ToDateFormatDetailDto(df));
        }

        public async Task<Result<DateFormatDetailDto>> UpdateDateFormatAsync(Guid dateFormatId, UpdateDateFormatRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<DateFormatDetailDto>(Error.Validation("Validation failed", errors));
            }

            var df = await _dateFormatRepository.GetByIdAsync(dateFormatId, cancellationToken);
            if (df == null)
                return Result.Failure<DateFormatDetailDto>(Error.NotFound("DateFormat", dateFormatId.ToString()));

            DateFormatMapper.UpdateEntity(df, request);
            _dateFormatRepository.Update(df);
            await _dateFormatRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("DateFormat updated: {DateFormatId}", dateFormatId);
            return Result.Success(DateFormatMapper.ToDateFormatDetailDto(df));
        }

        public async Task<Result> DeleteDateFormatAsync(Guid dateFormatId, CancellationToken cancellationToken = default)
        {
            var df = await _dateFormatRepository.GetByIdAsync(dateFormatId, cancellationToken);
            if (df == null)
                return Result.Failure(Error.NotFound("DateFormat", dateFormatId.ToString()));

            df.IsDeleted = true;
            _dateFormatRepository.Update(df);
            await _dateFormatRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("DateFormat soft deleted: {DateFormatId}", dateFormatId);
            return Result.Success();
        }
    }
}
