using System.Security.Claims;
using SpinTrack.Core.Entities.Auth;

namespace SpinTrack.Application.Features.Auth.Interfaces
{
    /// <summary>
    /// Service for JWT token generation and validation
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        int GetAccessTokenExpirationMinutes();
    }
}
