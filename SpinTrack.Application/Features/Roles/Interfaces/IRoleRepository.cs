using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Role;

namespace SpinTrack.Application.Features.Roles.Interfaces
{
    /// <summary>
    /// Repository interface for Role entity
    /// </summary>
    public interface IRoleRepository
    {
        // Read operations
        Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
        Task<bool> RoleNameExistsAsync(string roleName, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);
        Task<PagedResult<TResult>> QueryAsync<TResult>(
            QueryRequest request,
            Func<Role, TResult> mapper,
            CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(
            QueryRequest request,
            Func<Role, TResult> mapper,
            CancellationToken cancellationToken = default);

        // Write operations
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        void Update(Role role);
        void Delete(Role role);

        // Save changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}