namespace SpinTrack.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Request DTO for refreshing access token
    /// </summary>
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
