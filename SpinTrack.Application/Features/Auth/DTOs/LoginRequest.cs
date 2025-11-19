namespace SpinTrack.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Request DTO for user login
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
