using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Auth.DTOs;

namespace SpinTrack.Application.Features.Auth.Interfaces
{
    /// <summary>
    /// Service for authentication operations
    /// </summary>
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task<Result> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    }
}
