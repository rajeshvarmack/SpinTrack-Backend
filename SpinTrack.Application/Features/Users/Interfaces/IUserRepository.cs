using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Auth;

namespace SpinTrack.Application.Features.Users.Interfaces
{
    /// <summary>
    /// Repository interface for User entity with specific query methods
    /// </summary>
    public interface IUserRepository
    {
        // Read operations
        Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
        Task<PagedResult<TResult>> QueryAsync<TResult>(
            QueryRequest request,
            Func<User, TResult> mapper,
            CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(
            QueryRequest request,
            Func<User, TResult> mapper,
            CancellationToken cancellationToken = default);

        // Write operations
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        void Update(User user);
        void Delete(User user);
        
        // Save changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
