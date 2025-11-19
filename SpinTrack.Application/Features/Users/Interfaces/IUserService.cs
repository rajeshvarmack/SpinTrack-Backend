using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Auth.DTOs;
using SpinTrack.Application.Features.Users.DTOs;

namespace SpinTrack.Application.Features.Users.Interfaces
{
    /// <summary>
    /// Service for user management operations
    /// </summary>
    public interface IUserService
    {
        Task<Result<UserDetailDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<UserDetailDto>> CreateUserAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result<UserDetailDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Result> ChangeUserStatusAsync(Guid userId, ChangeUserStatusRequest request, CancellationToken cancellationToken = default);
    }
}
