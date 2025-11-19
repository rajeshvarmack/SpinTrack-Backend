using Microsoft.EntityFrameworkCore;
using SpinTrack.Application.Features.Auth.Interfaces;
using SpinTrack.Core.Entities.Auth;

namespace SpinTrack.Infrastructure.Repositories
{
    /// <summary>
    /// RefreshToken repository implementation using EF Core directly
    /// </summary>
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SpinTrackDbContext _context;

        public RefreshTokenRepository(SpinTrackDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            // AsNoTracking for read-only check, but we'll track it if found for potential updates
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
