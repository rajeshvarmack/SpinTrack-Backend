namespace SpinTrack.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Response DTO for authentication operations
    /// </summary>
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
