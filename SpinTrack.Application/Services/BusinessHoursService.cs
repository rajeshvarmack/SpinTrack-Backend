using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.BusinessHours.DTOs;
using SpinTrack.Application.Features.BusinessHours.Interfaces;
using SpinTrack.Application.Features.BusinessHours.Mappers;
using SpinTrack.Core.Entities.BusinessHours;

namespace SpinTrack.Application.Services
{
    public class BusinessHoursService : IBusinessHoursService
    {
        private readonly IBusinessHoursRepository _businessHoursRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateBusinessHoursRequest> _createValidator;
        private readonly IValidator<UpdateBusinessHoursRequest> _updateValidator;
        private readonly ILogger<BusinessHoursService> _logger;

        public BusinessHoursService(
            IBusinessHoursRepository businessHoursRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateBusinessHoursRequest> createValidator,
            IValidator<UpdateBusinessHoursRequest> updateValidator,
            ILogger<BusinessHoursService> logger)
        {
            _businessHoursRepository = businessHoursRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<BusinessHoursDetailDto>> GetBusinessHoursByIdAsync(Guid businessHoursId, CancellationToken cancellationToken = default)
        {
            var bh = await _businessHoursRepository.GetByIdAsync(businessHoursId, cancellationToken);
            if (bh == null)
                return Result.Failure<BusinessHoursDetailDto>(Error.NotFound("BusinessHours", businessHoursId.ToString()));

            return Result.Success(BusinessHoursMapper.ToBusinessHoursDetailDto(bh));
        }

        public async Task<Result<BusinessHoursDetailDto>> CreateBusinessHoursAsync(CreateBusinessHoursRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<BusinessHoursDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (request.EndTime <= request.StartTime)
                return Result.Failure<BusinessHoursDetailDto>(Error.Validation("EndTime must be greater than StartTime", null));

            if (await _businessHoursRepository.ExistsAsync(request.CompanyId, request.DayOfWeek, request.ShiftName, cancellationToken: cancellationToken))
            {
                return Result.Failure<BusinessHoursDetailDto>(Error.Conflict("Business hours for this company/day/shift already exists"));
            }

            var bh = BusinessHoursMapper.ToEntity(request);
            await _businessHoursRepository.AddAsync(bh, cancellationToken);
            await _businessHoursRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessHours created: {BusinessHoursId} {CompanyId} {Day} {Shift}", bh.BusinessHoursId, bh.CompanyId, bh.DayOfWeek, bh.ShiftName);
            return Result.Success(BusinessHoursMapper.ToBusinessHoursDetailDto(bh));
        }

        public async Task<Result<BusinessHoursDetailDto>> UpdateBusinessHoursAsync(Guid businessHoursId, UpdateBusinessHoursRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<BusinessHoursDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (request.EndTime <= request.StartTime)
                return Result.Failure<BusinessHoursDetailDto>(Error.Validation("EndTime must be greater than StartTime", null));

            var bh = await _businessHoursRepository.GetByIdAsync(businessHoursId, cancellationToken);
            if (bh == null)
                return Result.Failure<BusinessHoursDetailDto>(Error.NotFound("BusinessHours", businessHoursId.ToString()));

            BusinessHoursMapper.UpdateEntity(bh, request);
            _businessHoursRepository.Update(bh);
            await _businessHoursRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessHours updated: {BusinessHoursId}", businessHoursId);
            return Result.Success(BusinessHoursMapper.ToBusinessHoursDetailDto(bh));
        }

        public async Task<Result> DeleteBusinessHoursAsync(Guid businessHoursId, CancellationToken cancellationToken = default)
        {
            var bh = await _businessHoursRepository.GetByIdAsync(businessHoursId, cancellationToken);
            if (bh == null)
                return Result.Failure(Error.NotFound("BusinessHours", businessHoursId.ToString()));

            bh.IsDeleted = true;
            _businessHoursRepository.Update(bh);
            await _businessHoursRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessHours soft deleted: {BusinessHoursId}", businessHoursId);
            return Result.Success();
        }
    }
}
