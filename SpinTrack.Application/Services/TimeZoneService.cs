using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.TimeZones.DTOs;
using SpinTrack.Application.Features.TimeZones.Interfaces;
using SpinTrack.Application.Features.TimeZones.Mappers;
using SpinTrack.Core.Entities.TimeZone;

namespace SpinTrack.Application.Services
{
    public class TimeZoneService : ITimeZoneService
    {
        private readonly ITimeZoneRepository _timeZoneRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateTimeZoneRequest> _createValidator;
        private readonly IValidator<UpdateTimeZoneRequest> _updateValidator;
        private readonly ILogger<TimeZoneService> _logger;

        public TimeZoneService(
            ITimeZoneRepository timeZoneRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateTimeZoneRequest> createValidator,
            IValidator<UpdateTimeZoneRequest> updateValidator,
            ILogger<TimeZoneService> logger)
        {
            _timeZoneRepository = timeZoneRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<TimeZoneDetailDto>> GetTimeZoneByIdAsync(Guid timeZoneId, CancellationToken cancellationToken = default)
        {
            var tz = await _timeZoneRepository.GetByIdAsync(timeZoneId, cancellationToken);
            if (tz == null)
                return Result.Failure<TimeZoneDetailDto>(Error.NotFound("TimeZone", timeZoneId.ToString()));

            return Result.Success(TimeZoneMapper.ToTimeZoneDetailDto(tz));
        }

        public async Task<Result<TimeZoneDetailDto>> CreateTimeZoneAsync(CreateTimeZoneRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<TimeZoneDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _timeZoneRepository.TimeZoneNameExistsAsync(request.TimeZoneName, cancellationToken: cancellationToken))
            {
                return Result.Failure<TimeZoneDetailDto>(Error.Conflict("TimeZone name already exists"));
            }

            var tz = TimeZoneMapper.ToEntity(request);
            await _timeZoneRepository.AddAsync(tz, cancellationToken);
            await _timeZoneRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("TimeZone created: {TimeZoneId} Name: {Name}", tz.TimeZoneId, tz.TimeZoneName);
            return Result.Success(TimeZoneMapper.ToTimeZoneDetailDto(tz));
        }

        public async Task<Result<TimeZoneDetailDto>> UpdateTimeZoneAsync(Guid timeZoneId, UpdateTimeZoneRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<TimeZoneDetailDto>(Error.Validation("Validation failed", errors));
            }

            var tz = await _timeZoneRepository.GetByIdAsync(timeZoneId, cancellationToken);
            if (tz == null)
                return Result.Failure<TimeZoneDetailDto>(Error.NotFound("TimeZone", timeZoneId.ToString()));

            TimeZoneMapper.UpdateEntity(tz, request);
            _timeZoneRepository.Update(tz);
            await _timeZoneRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("TimeZone updated: {TimeZoneId}", timeZoneId);
            return Result.Success(TimeZoneMapper.ToTimeZoneDetailDto(tz));
        }

        public async Task<Result> DeleteTimeZoneAsync(Guid timeZoneId, CancellationToken cancellationToken = default)
        {
            var tz = await _timeZoneRepository.GetByIdAsync(timeZoneId, cancellationToken);
            if (tz == null)
                return Result.Failure(Error.NotFound("TimeZone", timeZoneId.ToString()));

            tz.IsDeleted = true;
            _timeZoneRepository.Update(tz);
            await _timeZoneRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("TimeZone soft deleted: {TimeZoneId}", timeZoneId);
            return Result.Success();
        }
    }
}
