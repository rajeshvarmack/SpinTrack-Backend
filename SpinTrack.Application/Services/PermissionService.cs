using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Permissions.DTOs;
using SpinTrack.Application.Features.Permissions.Interfaces;
using SpinTrack.Application.Features.Permissions.Mappers;
using SpinTrack.Core.Entities.Permission;

namespace SpinTrack.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreatePermissionRequest> _createValidator;
        private readonly IValidator<UpdatePermissionRequest> _updateValidator;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            IPermissionRepository permissionRepository,
            ICurrentUserService currentUserService,
            IValidator<CreatePermissionRequest> createValidator,
            IValidator<UpdatePermissionRequest> updateValidator,
            ILogger<PermissionService> logger)
        {
            _permissionRepository = permissionRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<PermissionDetailDto>> GetPermissionByIdAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            var p = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (p == null)
                return Result.Failure<PermissionDetailDto>(Error.NotFound("Permission", permissionId.ToString()));

            return Result.Success(PermissionMapper.ToPermissionDetailDto(p));
        }

        public async Task<Result<PermissionDetailDto>> CreatePermissionAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<PermissionDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _permissionRepository.PermissionKeyExistsAsync(request.PermissionKey, cancellationToken: cancellationToken))
            {
                return Result.Failure<PermissionDetailDto>(Error.Conflict("Permission key already exists"));
            }

            var p = PermissionMapper.ToEntity(request);
            await _permissionRepository.AddAsync(p, cancellationToken);
            await _permissionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Permission created: {PermissionId} Key: {Key}", p.PermissionId, p.PermissionKey);
            return Result.Success(PermissionMapper.ToPermissionDetailDto(p));
        }

        public async Task<Result<PermissionDetailDto>> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<PermissionDetailDto>(Error.Validation("Validation failed", errors));
            }

            var p = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (p == null)
                return Result.Failure<PermissionDetailDto>(Error.NotFound("Permission", permissionId.ToString()));

            PermissionMapper.UpdateEntity(p, request);
            _permissionRepository.Update(p);
            await _permissionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Permission updated: {PermissionId}", permissionId);
            return Result.Success(PermissionMapper.ToPermissionDetailDto(p));
        }

        public async Task<Result> DeletePermissionAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            var p = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (p == null)
                return Result.Failure(Error.NotFound("Permission", permissionId.ToString()));

            p.IsDeleted = true;
            _permissionRepository.Update(p);
            await _permissionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Permission soft deleted: {PermissionId}", permissionId);
            return Result.Success();
        }

        public async Task<Result> ChangePermissionStatusAsync(Guid permissionId, ChangePermissionStatusRequest request, CancellationToken cancellationToken = default)
        {
            var p = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (p == null)
                return Result.Failure(Error.NotFound("Permission", permissionId.ToString()));

            p.Status = request.Status;
            _permissionRepository.Update(p);
            await _permissionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Permission status changed: {PermissionId} to {Status}", permissionId, request.Status);
            return Result.Success();
        }
    }
}