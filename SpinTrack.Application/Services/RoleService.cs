using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Roles.DTOs;
using SpinTrack.Application.Features.Roles.Interfaces;
using SpinTrack.Application.Features.Roles.Mappers;
using SpinTrack.Core.Entities.Role;

namespace SpinTrack.Application.Services
{
    /// <summary>
    /// Role management service with soft delete
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateRoleRequest> _createValidator;
        private readonly IValidator<UpdateRoleRequest> _updateValidator;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IRoleRepository roleRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateRoleRequest> createValidator,
            IValidator<UpdateRoleRequest> updateValidator,
            ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<RoleDetailDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result.Failure<RoleDetailDto>(Error.NotFound("Role", roleId.ToString()));

            return Result.Success(RoleMapper.ToRoleDetailDto(role));
        }

        public async Task<Result<RoleDetailDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<RoleDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _roleRepository.RoleNameExistsAsync(request.RoleName, cancellationToken: cancellationToken))
            {
                return Result.Failure<RoleDetailDto>(Error.Conflict("Role name already exists"));
            }

            var role = RoleMapper.ToEntity(request);

            await _roleRepository.AddAsync(role, cancellationToken);
            await _roleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role created successfully: {RoleId}, Name: {RoleName}", role.RoleId, role.RoleName);

            return Result.Success(RoleMapper.ToRoleDetailDto(role));
        }

        public async Task<Result<RoleDetailDto>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<RoleDetailDto>(Error.Validation("Validation failed", errors));
            }

            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result.Failure<RoleDetailDto>(Error.NotFound("Role", roleId.ToString()));

            RoleMapper.UpdateEntity(role, request);
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role updated successfully: {RoleId}", roleId);

            return Result.Success(RoleMapper.ToRoleDetailDto(role));
        }

        public async Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result.Failure(Error.NotFound("Role", roleId.ToString()));

            // Soft delete
            role.IsDeleted = true;
            role.DeletedAt = DateTimeOffset.UtcNow;
            role.DeletedBy = _currentUserService.UserId;

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role soft deleted successfully: {RoleId}", roleId);
            return Result.Success();
        }

        public async Task<Result> ChangeRoleStatusAsync(Guid roleId, ChangeRoleStatusRequest request, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result.Failure(Error.NotFound("Role", roleId.ToString()));

            role.Status = request.Status;
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role status changed successfully: {RoleId}, New Status: {Status}", roleId, request.Status);
            return Result.Success();
        }
    }
}