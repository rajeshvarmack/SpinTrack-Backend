using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Modules.DTOs;
using SpinTrack.Application.Features.Modules.Interfaces;
using SpinTrack.Application.Features.Modules.Mappers;
using SpinTrack.Core.Entities.Module;

namespace SpinTrack.Application.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateModuleRequest> _createValidator;
        private readonly IValidator<UpdateModuleRequest> _updateValidator;
        private readonly ILogger<ModuleService> _logger;

        public ModuleService(
            IModuleRepository moduleRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateModuleRequest> createValidator,
            IValidator<UpdateModuleRequest> updateValidator,
            ILogger<ModuleService> logger)
        {
            _moduleRepository = moduleRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<ModuleDetailDto>> GetModuleByIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
        {
            var module = await _moduleRepository.GetByIdAsync(moduleId, cancellationToken);
            if (module == null)
                return Result.Failure<ModuleDetailDto>(Error.NotFound("Module", moduleId.ToString()));

            return Result.Success(ModuleMapper.ToModuleDetailDto(module));
        }

        public async Task<Result<ModuleDetailDto>> CreateModuleAsync(CreateModuleRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<ModuleDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _moduleRepository.ModuleKeyExistsAsync(request.ModuleKey, cancellationToken: cancellationToken))
            {
                return Result.Failure<ModuleDetailDto>(Error.Conflict("Module key already exists"));
            }

            var module = ModuleMapper.ToEntity(request);
            await _moduleRepository.AddAsync(module, cancellationToken);
            await _moduleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Module created successfully: {ModuleId}, Key: {ModuleKey}", module.ModuleId, module.ModuleKey);
            return Result.Success(ModuleMapper.ToModuleDetailDto(module));
        }

        public async Task<Result<ModuleDetailDto>> UpdateModuleAsync(Guid moduleId, UpdateModuleRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<ModuleDetailDto>(Error.Validation("Validation failed", errors));
            }

            var module = await _moduleRepository.GetByIdAsync(moduleId, cancellationToken);
            if (module == null)
                return Result.Failure<ModuleDetailDto>(Error.NotFound("Module", moduleId.ToString()));

            ModuleMapper.UpdateEntity(module, request);
            _moduleRepository.Update(module);
            await _moduleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Module updated successfully: {ModuleId}", moduleId);
            return Result.Success(ModuleMapper.ToModuleDetailDto(module));
        }

        public async Task<Result> DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
        {
            var module = await _moduleRepository.GetByIdAsync(moduleId, cancellationToken);
            if (module == null)
                return Result.Failure(Error.NotFound("Module", moduleId.ToString()));

            module.IsDeleted = true;
            _moduleRepository.Update(module);
            await _moduleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Module soft deleted: {ModuleId}", moduleId);
            return Result.Success();
        }

        public async Task<Result> ChangeModuleStatusAsync(Guid moduleId, ChangeModuleStatusRequest request, CancellationToken cancellationToken = default)
        {
            var module = await _moduleRepository.GetByIdAsync(moduleId, cancellationToken);
            if (module == null)
                return Result.Failure(Error.NotFound("Module", moduleId.ToString()));

            module.Status = request.Status;
            _moduleRepository.Update(module);
            await _moduleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Module status changed: {ModuleId} to {Status}", moduleId, request.Status);
            return Result.Success();
        }
    }
}