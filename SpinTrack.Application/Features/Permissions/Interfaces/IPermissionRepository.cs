using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Permission;

namespace SpinTrack.Application.Features.Permissions.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken = default);
        Task<Permission?> GetByKeyAsync(string permissionKey, CancellationToken cancellationToken = default);
        Task<bool> PermissionKeyExistsAsync(string permissionKey, Guid? excludePermissionId = null, CancellationToken cancellationToken = default);
        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Permission, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Permission, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
        void Update(Permission permission);
        void Delete(Permission permission);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}