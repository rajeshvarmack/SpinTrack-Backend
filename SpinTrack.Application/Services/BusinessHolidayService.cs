using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.BusinessHolidays.DTOs;
using SpinTrack.Application.Features.BusinessHolidays.Interfaces;
using SpinTrack.Application.Features.BusinessHolidays.Mappers;
using SpinTrack.Core.Entities.BusinessHoliday;

namespace SpinTrack.Application.Services
{
    public class BusinessHolidayService : IBusinessHolidayService
    {
        private readonly IBusinessHolidayRepository _businessHolidayRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateBusinessHolidayRequest> _createValidator;
        private readonly IValidator<UpdateBusinessHolidayRequest> _updateValidator;
        private readonly ILogger<BusinessHolidayService> _logger;

        public BusinessHolidayService(
            IBusinessHolidayRepository businessHolidayRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateBusinessHolidayRequest> createValidator,
            IValidator<UpdateBusinessHolidayRequest> updateValidator,
            ILogger<BusinessHolidayService> logger)
        {
            _businessHolidayRepository = businessHolidayRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<BusinessHolidayDetailDto>> GetBusinessHolidayByIdAsync(Guid businessHolidayId, CancellationToken cancellationToken = default)
        {
            var bh = await _businessHolidayRepository.GetByIdAsync(businessHolidayId, cancellationToken);
            if (bh == null)
                return Result.Failure<BusinessHolidayDetailDto>(Error.NotFound("BusinessHoliday", businessHolidayId.ToString()));

            return Result.Success(BusinessHolidayMapper.ToBusinessHolidayDetailDto(bh));
        }

        public async Task<Result<BusinessHolidayDetailDto>> CreateBusinessHolidayAsync(CreateBusinessHolidayRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<BusinessHolidayDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (request.StartTime.HasValue ^ request.EndTime.HasValue)
            {
                return Result.Failure<BusinessHolidayDetailDto>(Error.Validation("Both StartTime and EndTime must be provided for partial holidays", null));
            }

            if (await _businessHolidayRepository.ExistsAsync(request.CompanyId, request.HolidayDate, cancellationToken: cancellationToken))
            {
                return Result.Failure<BusinessHolidayDetailDto>(Error.Conflict("Business holiday for this company and date already exists"));
            }

            var bh = BusinessHolidayMapper.ToEntity(request);
            await _businessHolidayRepository.AddAsync(bh, cancellationToken);
            await _businessHolidayRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessHoliday created: {BusinessHolidayId} {CompanyId} {Date}", bh.BusinessHolidayId, bh.CompanyId, bh.HolidayDate);
            return Result.Success(BusinessHolidayMapper.ToBusinessHolidayDetailDto(bh));
        }

        public async Task<Result<BusinessHolidayDetailDto>> UpdateBusinessHolidayAsync(Guid businessHolidayId, UpdateBusinessHolidayRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<BusinessHolidayDetailDto>(Error.Validation("Validation failed", errors));
            }

            var bh = await _businessHolidayRepository.GetByIdAsync(businessHolidayId, cancellationToken);
            if (bh == null)
                return Result.Failure<BusinessHolidayDetailDto>(Error.NotFound("BusinessHoliday", businessHolidayId.ToString()));

            BusinessHolidayMapper.UpdateEntity(bh, request);
            _businessHolidayRepository.Update(bh);
            await _businessHolidayRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessHoliday updated: {BusinessHolidayId}", businessHolidayId);
            return Result.Success(BusinessHolidayMapper.ToBusinessHolidayDetailDto(bh));
        }

        public async Task<Result> DeleteBusinessHolidayAsync(Guid businessHolidayId, CancellationToken cancellationToken = default)
        {
            var bh = await _businessHolidayRepository.GetByIdAsync(businessHolidayId, cancellationToken);
            if (bh == null)
                return Result.Failure(Error.NotFound("BusinessHoliday", businessHolidayId.ToString()));

            bh.IsDeleted = true;
            _businessHolidayRepository.Update(bh);
            await _businessHolidayRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessHoliday soft deleted: {BusinessHolidayId}", businessHolidayId);
            return Result.Success();
        }
    }
}
