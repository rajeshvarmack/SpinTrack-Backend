using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Roles.DTOs;

namespace SpinTrack.Application.Features.Roles.Interfaces
{
    /// <summary>
    /// Service for role management
    /// </summary>
    public interface IRoleService
    {
        Task<Result<RoleDetailDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Result<RoleDetailDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
        Task<Result<RoleDetailDto>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Result> ChangeRoleStatusAsync(Guid roleId, ChangeRoleStatusRequest request, CancellationToken cancellationToken = default);
    }
}