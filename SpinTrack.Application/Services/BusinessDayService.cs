using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.BusinessDays.DTOs;
using SpinTrack.Application.Features.BusinessDays.Interfaces;
using SpinTrack.Application.Features.BusinessDays.Mappers;
using SpinTrack.Core.Entities.BusinessDay;

namespace SpinTrack.Application.Services
{
    public class BusinessDayService : IBusinessDayService
    {
        private readonly IBusinessDayRepository _businessDayRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateBusinessDayRequest> _createValidator;
        private readonly IValidator<UpdateBusinessDayRequest> _updateValidator;
        private readonly ILogger<BusinessDayService> _logger;

        public BusinessDayService(
            IBusinessDayRepository businessDayRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateBusinessDayRequest> createValidator,
            IValidator<UpdateBusinessDayRequest> updateValidator,
            ILogger<BusinessDayService> logger)
        {
            _businessDayRepository = businessDayRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<BusinessDayDetailDto>> GetBusinessDayByIdAsync(Guid businessDayId, CancellationToken cancellationToken = default)
        {
            var bd = await _businessDayRepository.GetByIdAsync(businessDayId, cancellationToken);
            if (bd == null)
                return Result.Failure<BusinessDayDetailDto>(Error.NotFound("BusinessDay", businessDayId.ToString()));

            return Result.Success(BusinessDayMapper.ToBusinessDayDetailDto(bd));
        }

        public async Task<Result<BusinessDayDetailDto>> CreateBusinessDayAsync(CreateBusinessDayRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<BusinessDayDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _businessDayRepository.ExistsAsync(request.CompanyId, request.DayOfWeek, cancellationToken: cancellationToken))
            {
                return Result.Failure<BusinessDayDetailDto>(Error.Conflict("Business day for this company and day already exists"));
            }

            var bd = BusinessDayMapper.ToEntity(request);
            await _businessDayRepository.AddAsync(bd, cancellationToken);
            await _businessDayRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessDay created: {BusinessDayId} {CompanyId} {Day}", bd.BusinessDayId, bd.CompanyId, bd.DayOfWeek);
            return Result.Success(BusinessDayMapper.ToBusinessDayDetailDto(bd));
        }

        public async Task<Result<BusinessDayDetailDto>> UpdateBusinessDayAsync(Guid businessDayId, UpdateBusinessDayRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<BusinessDayDetailDto>(Error.Validation("Validation failed", errors));
            }

            var bd = await _businessDayRepository.GetByIdAsync(businessDayId, cancellationToken);
            if (bd == null)
                return Result.Failure<BusinessDayDetailDto>(Error.NotFound("BusinessDay", businessDayId.ToString()));

            BusinessDayMapper.UpdateEntity(bd, request);
            _businessDayRepository.Update(bd);
            await _businessDayRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessDay updated: {BusinessDayId}", businessDayId);
            return Result.Success(BusinessDayMapper.ToBusinessDayDetailDto(bd));
        }

        public async Task<Result> DeleteBusinessDayAsync(Guid businessDayId, CancellationToken cancellationToken = default)
        {
            var bd = await _businessDayRepository.GetByIdAsync(businessDayId, cancellationToken);
            if (bd == null)
                return Result.Failure(Error.NotFound("BusinessDay", businessDayId.ToString()));

            bd.IsDeleted = true;
            _businessDayRepository.Update(bd);
            await _businessDayRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BusinessDay soft deleted: {BusinessDayId}", businessDayId);
            return Result.Success();
        }
    }
}
