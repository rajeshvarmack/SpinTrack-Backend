namespace SpinTrack.Core.Entities.Auth
{
    /// <summary>
    /// Refresh token entity for JWT token refresh mechanism
    /// </summary>
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;

        // Helper properties
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
