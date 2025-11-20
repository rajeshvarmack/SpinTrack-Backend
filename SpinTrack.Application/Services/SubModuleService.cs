using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.SubModules.DTOs;
using SpinTrack.Application.Features.SubModules.Interfaces;
using SpinTrack.Application.Features.SubModules.Mappers;
using SpinTrack.Core.Entities.SubModule;

namespace SpinTrack.Application.Services
{
    public class SubModuleService : ISubModuleService
    {
        private readonly ISubModuleRepository _subModuleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateSubModuleRequest> _createValidator;
        private readonly IValidator<UpdateSubModuleRequest> _updateValidator;
        private readonly ILogger<SubModuleService> _logger;

        public SubModuleService(
            ISubModuleRepository subModuleRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateSubModuleRequest> createValidator,
            IValidator<UpdateSubModuleRequest> updateValidator,
            ILogger<SubModuleService> logger)
        {
            _subModuleRepository = subModuleRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<SubModuleDetailDto>> GetSubModuleByIdAsync(Guid subModuleId, CancellationToken cancellationToken = default)
        {
            var sub = await _subModuleRepository.GetByIdAsync(subModuleId, cancellationToken);
            if (sub == null)
                return Result.Failure<SubModuleDetailDto>(Error.NotFound("SubModule", subModuleId.ToString()));

            return Result.Success(SubModuleMapper.ToSubModuleDetailDto(sub));
        }

        public async Task<Result<SubModuleDetailDto>> CreateSubModuleAsync(CreateSubModuleRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<SubModuleDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _subModuleRepository.SubModuleKeyExistsAsync(request.SubModuleKey, cancellationToken: cancellationToken))
            {
                return Result.Failure<SubModuleDetailDto>(Error.Conflict("SubModule key already exists"));
            }

            var sub = SubModuleMapper.ToEntity(request);
            await _subModuleRepository.AddAsync(sub, cancellationToken);
            await _subModuleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("SubModule created: {SubModuleId}", sub.SubModuleId);
            return Result.Success(SubModuleMapper.ToSubModuleDetailDto(sub));
        }

        public async Task<Result<SubModuleDetailDto>> UpdateSubModuleAsync(Guid subModuleId, UpdateSubModuleRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<SubModuleDetailDto>(Error.Validation("Validation failed", errors));
            }

            var sub = await _subModuleRepository.GetByIdAsync(subModuleId, cancellationToken);
            if (sub == null)
                return Result.Failure<SubModuleDetailDto>(Error.NotFound("SubModule", subModuleId.ToString()));

            SubModuleMapper.UpdateEntity(sub, request);
            _subModuleRepository.Update(sub);
            await _subModuleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("SubModule updated: {SubModuleId}", subModuleId);
            return Result.Success(SubModuleMapper.ToSubModuleDetailDto(sub));
        }

        public async Task<Result> DeleteSubModuleAsync(Guid subModuleId, CancellationToken cancellationToken = default)
        {
            var sub = await _subModuleRepository.GetByIdAsync(subModuleId, cancellationToken);
            if (sub == null)
                return Result.Failure(Error.NotFound("SubModule", subModuleId.ToString()));

            sub.IsDeleted = true;
            _subModuleRepository.Update(sub);
            await _subModuleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("SubModule soft deleted: {SubModuleId}", subModuleId);
            return Result.Success();
        }

        public async Task<Result> ChangeSubModuleStatusAsync(Guid subModuleId, ChangeSubModuleStatusRequest request, CancellationToken cancellationToken = default)
        {
            var sub = await _subModuleRepository.GetByIdAsync(subModuleId, cancellationToken);
            if (sub == null)
                return Result.Failure(Error.NotFound("SubModule", subModuleId.ToString()));

            sub.Status = request.Status;
            _subModuleRepository.Update(sub);
            await _subModuleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("SubModule status changed: {SubModuleId} to {Status}", subModuleId, request.Status);
            return Result.Success();
        }
    }
}