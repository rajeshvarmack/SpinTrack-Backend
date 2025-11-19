using SpinTrack.Core.Entities.Auth;

namespace SpinTrack.Application.Features.Auth.Interfaces
{
    /// <summary>
    /// Repository interface for RefreshToken entity
    /// </summary>
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        void Update(RefreshToken refreshToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
