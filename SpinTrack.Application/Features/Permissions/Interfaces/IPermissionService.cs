using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Permissions.DTOs;

namespace SpinTrack.Application.Features.Permissions.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<PermissionDetailDto>> GetPermissionByIdAsync(Guid permissionId, CancellationToken cancellationToken = default);
        Task<Result<PermissionDetailDto>> CreatePermissionAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default);
        Task<Result<PermissionDetailDto>> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeletePermissionAsync(Guid permissionId, CancellationToken cancellationToken = default);
        Task<Result> ChangePermissionStatusAsync(Guid permissionId, ChangePermissionStatusRequest request, CancellationToken cancellationToken = default);
    }
}